using AutoMapper;
using Repository.DataRepositories;
using Repository.Interfaces;
using Repository.models;
using Service.Dto;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class MatchService: IMatch
    {
        private readonly IRepository<Match> _repository;
        private readonly IRepository<CandidateProfiles> _candidateRepository;
        private readonly IRepository<JobListings> _jobRepository;
        private readonly IMapper mapper;

        // מילון לשמירת תוצאות ביניים (Memoization)
        private Dictionary<string, double> _memo = new Dictionary<string, double>();
        // מילון לשמירת הבחירה הכי טובה בכל שלב (כדי לשחזר את השיבוץ)
        private Dictionary<string, int> _bestJobChoice = new Dictionary<string, int>();

        public MatchService(IRepository<Match> matchRepository,
                            IRepository<CandidateProfiles> candidateRepository,
                            IRepository<JobListings> jobRepository,
                            IMapper mapper)
        {
            _repository = matchRepository;
            _candidateRepository = candidateRepository;
            _jobRepository = jobRepository;
            this.mapper = mapper;
        }
        public  async Task<MatchDto> AddItem(MatchDto item)
        {
            return mapper.Map<Match, MatchDto>(

            await _repository.AddItem(mapper.Map<MatchDto, Match>(item)));
        }

        public async Task<double> CalculateMatchScore(int candidateId, int jobId)
        {
            var candidate = await _candidateRepository.GetById(candidateId);
            var job = await _jobRepository.GetById(jobId);

            // 1. סינון ראשוני (Hard Filtering)
            // אם המשרה תפוסה, המועמד לא פעיל, או שהם בכלל לא מאותה קטגוריה - אין טעם להמשיך
            if (candidate == null || job == null || job.IsCatch || !candidate.activity)
                return 0;

            // בדיקת קטגוריה: אם המועמד והמשרה לא באותו תחום, הציון הוא 0
            if (candidate.CategoryId != job.CategoryId)
                return 0;

            double score = 0;

            // 2. בדיקת מיקום ומרחק
            if (job.IsRemote)
            {
                score += 20; // בונוס על משרה מרחוק (חוסך נסיעות)
            }
            else
            {
                // כאן נכנסת הלוגיקה של המרחק. 
                // הערה: בפרויקט גמר, אם אין לך API של מפות, בדרך כלל מניחים שערים זהות = מרחק 0
                if (candidate.City == job.Location)
                {
                    score += 20; // אותה עיר - התאמה מצוינת
                }
                // אם תרצי להוסיף חישוב מרחק אמיתי בין ערים, זה המקום
            }

            // 3. התאמת רמת קושי (Level)
            if (candidate.level == job.leveJob)
            {
                score += 30;
            }
            else if (Math.Abs((int)candidate.level - (int)job.leveJob) == 1)
            {
                score += 15;
            }

            // 4. עבודה מהבית (IsRemoteOnly)
            if (candidate.IsRemoteOnly && job.IsRemote)
                score += 20;
            else if (!candidate.IsRemoteOnly)
                score += 10; // גמישות המועמד שווה נקודות

            // 5. עבודה עם אנשים
            if (candidate.Withpepole == job.IsJobWithPepole)
                score += 20;

            // 6. שכר
            if (job.Payment >= candidate.MinHourlyRate)
                score += 10;

            return score;
        }


        public  async Task DeleteItem(int id)
        {
            await _repository.DeleteItem(id);
        }

        public async Task<List<MatchDto>> GetAll()
        {
            return mapper.Map<List<Match>, List<MatchDto>>(await
                                _repository.GetAll()); throw new NotImplementedException();
        }

        public async Task<MatchDto> GetById(int id)
        {
            return mapper.Map<Match, MatchDto>(await _repository.GetById(id));
        }

        //public Task<List<MatchDto>> GetTopMatchesForCandidate(int candidateId, int topCount)
        //{


        //}
        public async Task<List<MatchDto>> GetTopMatchesForCandidate(int candidateId, int topCount)
        {
            // שליפת כל ההתאמות של המועמד מהדאטה-בייס
            var allMatches = await _repository.GetAll();

            // סינון: אנחנו רוצים רק את ההתאמה שהאלגוריתם הדינמי בחר כ"הכי טובה למערכת"
            var bestMatch = allMatches
                .Where(m => m.CandidateId == candidateId && m.IsSelectedByAlgorithm == true)
                .OrderByDescending(m => m.MatchScore)
                .Take(1) // לוקחים רק אחד, כפי שביקשת
                .ToList();

            // אם במקרה האלגוריתם עוד לא רץ או לא מצא שידוך אופטימלי, 
            // אפשר להחזיר את ההתאמה הכי גבוהה באופן כללי כברירת מחדל
            if (!bestMatch.Any())
            {
                bestMatch = allMatches
                    .Where(m => m.CandidateId == candidateId)
                    .OrderByDescending(m => m.MatchScore)
                    .Take(1)
                    .ToList();
            }

            return mapper.Map<List<Match>, List<MatchDto>>(bestMatch);
        }
        public async Task<List<MatchDto>> RunMatchingAlgorithm(int dummy)
        {
            var candidates = (await _candidateRepository.GetAll()).ToList();
            var jobs = (await _jobRepository.GetAll()).ToList();
            int n = candidates.Count;
            int m = jobs.Count;

            // בניית מטריצת ציונים (מועמדים מול משרות)
            double[,] matrix = new double[n, m];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    matrix[i, j] = await CalculateMatchScore(candidates[i].Id, jobs[j].Id);

            _memo.Clear();
            _bestJobChoice.Clear();

            // הפעלת ה-DP
            await SolveDP(0, 0, matrix, n, m);

            // שחזור הבחירות האופטימליות מהזיכרון
            List<Match> finalMatches = new List<Match>();
            int currentMask = 0;
            for (int i = 0; i < n; i++)
            {
                string state = $"{i}-{currentMask}";
                if (_bestJobChoice.ContainsKey(state))
                {
                    int chosenJobIdx = _bestJobChoice[state];
                    if (chosenJobIdx != -1) // -1 אומר שלא נמצא שידוך משתלם
                    {
                        finalMatches.Add(new Match
                        {
                            CandidateId = candidates[i].Id,
                            JobId = jobs[chosenJobIdx].Id,
                            MatchScore = matrix[i, chosenJobIdx],
                            MatchDate = DateTime.Now,
                            IsSelectedByAlgorithm = true
                        });
                        currentMask |= (1 << chosenJobIdx); // סימון המשרה כתפוסה
                    }
                }
            }

            // שמירה ל-DB (מומלץ למחוק שיבוצים קודמים קודם)
            foreach (var match in finalMatches)
            {
                await _repository.AddItem(match);
            }

            return mapper.Map<List<Match>, List<MatchDto>>(finalMatches);
        }

        public async Task<double> SolveDP(int candIdx, int jobMask, double[,] matrix, int n, int m)
        {
            if (candIdx == n) return 0;

            string state = $"{candIdx}-{jobMask}";
            if (_memo.ContainsKey(state)) return _memo[state];

            // אפשרות א': המועמד הנוכחי לא משובץ
            double bestScore = await SolveDP(candIdx + 1, jobMask, matrix, n, m);
            int bestJob = -1;

            // אפשרות ב': לנסות לשבץ לכל משרה פנויה
            for (int j = 0; j < m; j++)
            {
                // בדיקה אם המשרה j פנויה בביטמאסק
                if ((jobMask & (1 << j)) == 0)
                {
                    // ציון = (עצם השיבוץ כדי למנוע אבטלה) + (התאמה למשרה)
                    double currentScore = (100 + matrix[candIdx, j]) +
                                          await SolveDP(candIdx + 1, jobMask | (1 << j), matrix, n, m);

                    if (currentScore > bestScore)
                    {
                        bestScore = currentScore;
                        bestJob = j;
                    }
                }
            }

            _bestJobChoice[state] = bestJob;
            return _memo[state] = bestScore;
        }
        public async Task<double> GetGlobalSatisfactionRate()
        {
            var allFinalMatches = (await _repository.GetAll())
                                  .Where(m => m.IsSelectedByAlgorithm == true)
                                  .ToList();

            if (!allFinalMatches.Any()) return 0;

            // ממוצע אחוזי ההתאמה של כל מי ששובץ
            double averageScore = allFinalMatches.Average(m => m.MatchScore);

            return averageScore;
        }

        public async Task UpdateItem(int id, MatchDto item)
        {
            var MatchEntity = mapper.Map<MatchDto, Match>(item);

            // 2. שולחים לרפוסיטורי את ה-ID ואת הישות הממופת
            await _repository.UpdateItem(id, MatchEntity);
        }
    }
}
