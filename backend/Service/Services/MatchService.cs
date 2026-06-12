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
    public class MatchService(
        IRepository<Match> matchRepository,
        IRepositoryCandidateProfiles candidateRepository,
        IRepository<JobListings> jobRepository,
        IMapper mapper) : IMatch
    {
        private readonly IRepository<Match> _repository = matchRepository;
        private readonly IRepositoryCandidateProfiles _candidateRepository = candidateRepository;
        private readonly IRepository<JobListings> _jobRepository = jobRepository;
        private readonly IMapper _mapper = mapper;
        // מילון לשמירת תוצאות ביניים (Memoization)
        private Dictionary<string, double> _memo = new Dictionary<string, double>();
        // מילון לשמירת הבחירה הכי טובה בכל שלב (כדי לשחזר את השיבוץ)
        private Dictionary<string, int> _bestJobChoice = new Dictionary<string, int>();

        public async Task<MatchDto> AddItem(MatchDto item)
        {
            // 1. המרת ה-DTO לישות (Entity) כדי שנוכל לעבוד איתה
            var matchEntity = _mapper.Map<Match>(item);

            // 2. בדיקה האם כבר קיימת התאמה בין המועמד הספציפי למשרה הספציפית
            var allMatches = await _repository.GetAll();
            var existingMatch = allMatches.FirstOrDefault(m =>
                m.CandidateId == matchEntity.CandidateId &&
                m.JobId == matchEntity.JobId);

            // 3. אם לא קיים - מוסיפים. אם קיים - מחזירים את הקיים (או מעדכנים)
            if (existingMatch == null)
            {
                var addedMatch = await _repository.AddItem(matchEntity);
                return _mapper.Map<MatchDto>(addedMatch);
            }

            // מחזירים את ה-Match הקיים כדי למנוע כפילויות ב-DB
            return _mapper.Map<MatchDto>(existingMatch);
        }

        public async Task<MatchDto> GetMatchByJC(int idJob, int idCandidate)
        {
            var allMatches = await GetAll();
            var match = allMatches.FirstOrDefault(m => m.JobId == idJob && m.CandidateId == idCandidate);
            if (match != null)
                return _mapper.Map<MatchDto>(match);
            return null;
        }

        public async Task<List<MatchDto>> GetMatchsByEmpID(int idEmp)
        {
            var allJobs = await _jobRepository.GetAll();
            var jobsEmp = allJobs.Where(j => j.EmployerId == idEmp).ToList(); // Filter jobs by EmployerId

            var allMatches = await _repository.GetAll();
            var filteredMatches = allMatches.Where(m => jobsEmp.Any(job => job.Id == m.JobId)).ToList(); // Correctly check if JobId exists in jobsEmp

            var matchDtos = _mapper.Map<List<Match>, List<MatchDto>>(filteredMatches);

            foreach (var dto in matchDtos)
            {
                var jobDto = allJobs.FirstOrDefault(j => j.Id == dto.JobId);
                if (jobDto != null)
                {
                    dto.Job = _mapper.Map<JobListingsDto>(jobDto); // Correctly map JobListings to JobListingsDto
                }
            }

            return matchDtos;
        }
        public async Task DeleteItem(int id)
        {
            await _repository.DeleteItem(id);
        }

        public async Task<List<MatchDto>> GetAll()
        {
            return mapper.Map<List<Match>, List<MatchDto>>(await _repository.GetAll());
        }

        public async Task<MatchDto> GetById(int id)
        {
            return mapper.Map<Match, MatchDto>(await _repository.GetById(id));
        }

        public async Task UpdateItem(int id, MatchDto item)
        {
            var MatchEntity = mapper.Map<MatchDto, Match>(item);
            await _repository.UpdateItem(id, MatchEntity);
        }


        public async Task<bool> ApplyForJob(int candidateId, int jobId)
        {
            // 1. בדיקה אם כבר קיים Match (אולי האלגוריתם כבר הציע לו את המשרה)
            var allMatches = await _repository.GetAll();
            var existingMatch = allMatches.FirstOrDefault(m => m.CandidateId == candidateId && m.JobId == jobId);

            if (existingMatch != null)
            {
                // אם קיים - פשוט מעדכנים סטטוס ל"הוגש" (Applied / Accepted)
                existingMatch.Status = "Applied";
                existingMatch.MatchDate = DateTime.Now;
                await _repository.UpdateItem(existingMatch.Id, existingMatch);
                return true;
            }

            // 2. אם לא קיים Match - יוצרים אחד חדש ביוזמת המועמד
            var newMatch = new Match
            {
                CandidateId = candidateId,
                JobId = jobId,
                MatchDate = DateTime.Now,
                Status = "Applied",
                IsSelectedByAlgorithm = false, // זה לא בא מהאלגוריתם, זה מהמועמד
                MatchScore = 0 // נחשב ציון בכל זאת
            };

            await _repository.AddItem(newMatch);
            return true;
        }

        public async Task<double> CalculateScorEmp(int candidateId, int jobId)
        {
            // 1. שליפת המידע מה-DB
            var candidate = await _candidateRepository.GetById(candidateId);
            var job = await _jobRepository.GetById(jobId);

            if (candidate == null || job == null) return 0;

            double score = 0;
            int criteriaCount = 0;

            // 2. השוואת קטגוריה (משקל גבוה מאוד)
            criteriaCount++;
            if (candidate.CategoryId == job.CategoryId) score += 40;

            // 3. השוואת רמת קושי/ניסיון (elevel)
            criteriaCount++;
            if (candidate.level == job.leveJob) score += 20;
            else if (Math.Abs((int)candidate.level - (int)job.leveJob) == 1) score += 10; // קרוב

            // 4. עבודה מרחוק (IsRemote)
            criteriaCount++;
            if (candidate.IsRemoteOnly && job.IsRemote) score += 20;
            else if (!candidate.IsRemoteOnly) score += 10; // המועמד גמיש

            // 5. שכר (MinHourlyRate)
            if (job.Payment.HasValue && candidate.MinHourlyRate.HasValue)
            {
                criteriaCount++;
                if (job.Payment >= candidate.MinHourlyRate) score += 20;
            }

            // 6. הפחתת ניקוד אם המועמד עדכן את הפרופיל לאחר הגשת ההתאמה
            // נניח שיש עמודות CandidateProfiles.ProfileUpdatedDate ו-Match.MatchDate
            // אם ProfileUpdatedDate > MatchDate, הפחת ניקוד (למשל 20%)
            var match = await _repository.GetAll();
            var relevantMatch = match.FirstOrDefault(m => m.CandidateId == candidate.Id && m.JobId == jobId);

            if (relevantMatch != null)
            {
                var profileUpdatedDateProp = candidate.GetType().GetProperty("ProfileUpdatedDate");
                var matchDateProp = relevantMatch.GetType().GetProperty("MatchDate");
                if (profileUpdatedDateProp != null && matchDateProp != null)
                {
                    var profileUpdatedDate = profileUpdatedDateProp.GetValue(candidate) as DateTime?;
                    var matchDate = matchDateProp.GetValue(relevantMatch) as DateTime?;
                    if (profileUpdatedDate.HasValue && matchDate.HasValue && profileUpdatedDate > matchDate)
                    {
                        score *= 0.8; // הפחתה של 20%
                    }
                }
            }

            return score; // ציון מתוך 100
        }
        public async Task<MatchDto> MostMatch(int jobId)
        {
            var job = await _jobRepository.GetById(jobId);

            var matchesCan = (await _repository.GetAll())
                                            .Where(m => m.JobId == jobId)
                                            .ToHashSet(); // HashSet for quick lookup

            var suggestions = new List<MatchDto>();

            foreach (var can in matchesCan)
            {
                // Quick in-memory check instead of another DB call
                double score = await CalculateScorEmp(can.CandidateId, jobId);

                // Create DTO including job details + score
                var suggestion = new MatchDto
                {
                    
                    JobId = jobId,
                    CandidateId = can.CandidateId,
                    MatchScore = score,
                    Status = "rejected",
                    MatchDate = DateTime.Now,
                };
                suggestions.Add(suggestion);
            }

            // Sort descending by MatchScore and take the top suggestion
            var topSuggestion = suggestions.OrderByDescending(s => s.MatchScore).FirstOrDefault();
            var top = await GetMatchByJC(topSuggestion.JobId, topSuggestion.CandidateId);
            if (top != null)
            {
                topSuggestion.Id = top.Id;
                topSuggestion.Candidate = top.Candidate;
                topSuggestion.JobId = top.JobId;
                topSuggestion.MatchDate = DateTime.Now;
                
                await UpdateItem(top.Id, topSuggestion);
            }
            return topSuggestion;
        }
        // 3. מיון יורד לפי Score ולקיחת 20 הראשונים
        public async Task<List<MatchDto>> GetRejecteds(int idEmp)
        {
            var allMachesEmp = await GetMatchsByEmpID(idEmp);
            return allMachesEmp.Where(a => a.Status == "rejected").ToList();
        }

    }

}


