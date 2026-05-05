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
    public class MatchService : IMatch
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
        public async Task<MatchDto> AddItem(MatchDto item)
        {
            return mapper.Map<Match, MatchDto>(

            await _repository.AddItem(mapper.Map<MatchDto, Match>(item)));
        }

        public async Task<double> CalculateMatchScore(int candidateId, int jobId)
        {
            try
            {
                var candidate = await _candidateRepository.GetById(candidateId);
                var job = await _jobRepository.GetById(jobId);

                // 1. סינון ראשוני (Hard Filtering)
                // אם המשרה תפוסה, המועמד לא פעיל, או שהם בכלל לא מאותה קטגוריה - אין טעם להמשיך
                if (candidate == null || job == null || job.IsCatch || !candidate.activity)
                    return 0;

                // בדיקת קטגוריה: אם המועמד והמשרה לא באותו תחום, הציון הוא 0
                // טיפול ב-NULL ב-CategoryId - אם למועמד אין קטגוריה, משתמשים בברירת המחדל (3)
                int candidateCategoryId = candidate.CategoryId ?? 3; // ברירת מחדל לקטגוריה
                if (job.CategoryId == null || candidateCategoryId != job.CategoryId)
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
                    // טיפול ב-NULL בערים
                    if (!string.IsNullOrEmpty(candidate.City) && !string.IsNullOrEmpty(job.Location) && candidate.City == job.Location)
                    {
                        score += 20; // אותה עיר - התאמה מצוינת
                    }
                    // אם תרצי להוסיף חישוב מרחק אמיתי בין ערים, זה המקום
                }

                // 3. התאמת רמת קושי (Level)
                // טיפול ב-NULL ברמות
                if (candidate.level != null && job.leveJob != null)
                {
                    if (candidate.level == job.leveJob)
                    {
                        score += 30;
                    }
                    else if (Math.Abs((int)candidate.level - (int)job.leveJob) == 1)
                    {
                        score += 15;
                    }
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
                // טיפול ב-NULL בשכר
                if (candidate.MinHourlyRate == null || job.Payment >= candidate.MinHourlyRate)
                    score += 10;

                return score;
            }
            catch
            {
                return 0; // אם יש כל שגיאה, מחזירים ציון 0
            }
        }


        public async Task DeleteItem(int id)
        {
            await _repository.DeleteItem(id);
        }

        public async Task<List<MatchDto>> GetAll()
        {
            return mapper.Map<List<Match>, List<MatchDto>>(await
                                _repository.GetAll());
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
            try
            {
                var candidates = (await _candidateRepository.GetAll()).ToList();
                var jobs = (await _jobRepository.GetAll()).ToList();

                // בדיקת NULL ומסנון נתונים לפני עיבוד
                var validCandidates = candidates.Where(c => c != null && c.activity == true).ToList();
                var validJobs = jobs.Where(j => j != null && !j.IsCatch).ToList();

                int n = validCandidates.Count;
                int m = validJobs.Count;

                // אם אין מועמדים או משרות תקפות, מחזירים רשימה ריקה
                if (n == 0 || m == 0)
                {
                    return new List<MatchDto>();
                }

                // בניית מטריצת ציונים (מועמדים מול משרות)
                double[,] matrix = new double[n, m];
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        try
                        {
                            matrix[i, j] = await CalculateMatchScore(validCandidates[i].Id, validJobs[j].Id);
                        }
                        catch
                        {
                            matrix[i, j] = 0; // אם יש שגיאה בחישוב ציון, מקבעים ל-0
                        }
                    }
                }

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
                                CandidateId = validCandidates[i].Id,
                                JobId = validJobs[chosenJobIdx].Id,
                                MatchScore = matrix[i, chosenJobIdx],
                                MatchDate = DateTime.Now,
                                IsSelectedByAlgorithm = true,
                                Status = "pending" // סטטוס התחלתי גם למאצ'ים מהאלגוריתם
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
            catch (Exception ex)
            {
                // לוג של השגיאה והחזרת רשימה ריקה כדי לא לקרוס את המערכת
                Console.WriteLine($"Error in RunMatchingAlgorithm: {ex.Message}");
                return new List<MatchDto>();
            }
        }

        // הפעלת אלגוריתם השיבוץ רק עבור מועמד ספציפי לפי userId
        public async Task<List<MatchDto>> RunMatchingAlgorithmForUser(int userId)
        {
            try
            {
                Console.WriteLine($"🔄 Running matching algorithm for user {userId}");

                // מציאת הפרופיל של המועמד לפי userId
                var candidate = (await _candidateRepository.GetAll())
                    .FirstOrDefault(c => c.UserId == userId);

                if (candidate == null)
                {
                    Console.WriteLine($"❌ No candidate profile found for user {userId}");
                    return new List<MatchDto>();
                }

                Console.WriteLine($"✅ Found candidate profile: {candidate.Id} for user {userId}");

                var jobs = (await _jobRepository.GetAll()).ToList();
                var validJobs = jobs.Where(j => j != null && !j.IsCatch).ToList();

                if (!validJobs.Any())
                {
                    Console.WriteLine("❌ No valid jobs found");
                    return new List<MatchDto>();
                }

                // חישוב ציונים רק עבור המועמד הספציפי
                List<Match> candidateMatches = new List<Match>();

                foreach (var job in validJobs)
                {
                    try
                    {
                        double score = await CalculateMatchScore(candidate.Id, job.Id);
                        Console.WriteLine($"📊 Score for candidate {candidate.Id} with job {job.Id}: {score}");

                        if (score > 0) // רק אם יש התאמה
                        {
                            // בדיקת כפילות - האם כבר קיים מאץ' בין המועמד למשרה?
                            var existingMatch = await _repository.GetAll();
                            var isDuplicate = existingMatch.Any(m =>
                                m.CandidateId == candidate.Id &&
                                m.JobId == job.Id);

                            if (!isDuplicate)
                            {
                                candidateMatches.Add(new Match
                                {
                                    CandidateId = candidate.Id,
                                    JobId = job.Id,
                                    MatchScore = score,
                                    MatchDate = DateTime.Now,
                                    IsSelectedByAlgorithm = true,
                                    Status = "pending"
                                });
                            }
                            else
                            {
                                Console.WriteLine($"⚠️ Duplicate match avoided: Candidate {candidate.Id} -> Job {job.Id}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Error calculating score for job {job.Id}: {ex.Message}");
                    }
                }

                // מיון לפי ציון והחזרת התוצאות הטובות ביותר
                var finalMatches = candidateMatches
                    .OrderByDescending(m => m.MatchScore)
                    .Take(10) // הגבלה ל-10 התאמות הטובות
                    .ToList();

                // שמירה ל-DB
                foreach (var match in finalMatches)
                {
                    await _repository.AddItem(match);
                    Console.WriteLine($"💾 Saved match: Candidate {match.CandidateId} -> Job {match.JobId} (Score: {match.MatchScore})");
                }

                Console.WriteLine($"✅ Created {finalMatches.Count} matches for user {userId}");
                return mapper.Map<List<Match>, List<MatchDto>>(finalMatches);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in RunMatchingAlgorithmForUser: {ex.Message}");
                return new List<MatchDto>();
            }
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

        public  async Task<double> GetSatisfactionScore()
        {
            // שליפת כל המאצ'ים מהמסד נתונים
            var allMatches = await _repository.GetAll();

            // אם אין מאצ'ים בכלל, נחזיר 0 כדי למנוע חלוקה באפס
            if (allMatches == null || !allMatches.Any())
            {
                return 0;
            }

            // חישוב ממוצע של כל ה-MatchScore במערכת
            double averageScore = allMatches.Average(m => m.MatchScore);

            return averageScore;
        }
    }
}
