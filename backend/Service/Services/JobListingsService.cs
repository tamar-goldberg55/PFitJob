using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    public class JobListingsService : IJobListings
    {
        private readonly IRepository<JobListings> _repository;
        private readonly IRepositoryCandidateProfiles _candidateRepository;
        private readonly IMapper mapper;
        private readonly IMatch _matchService;
        private readonly JobListingsExtendedRepository _extendedRepository; // Repository מורחב עם Include

        public JobListingsService(IRepository<JobListings> repository, IMapper map, JobListingsExtendedRepository extendedRepository, IRepositoryCandidateProfiles candidateRepository, IMatch matchService)
        {
            _repository = repository;
            mapper = map;
            _extendedRepository = extendedRepository; // שימוש ב-Repository המורחב
            _candidateRepository = candidateRepository;
            _matchService = matchService;
        }

        public async Task<JobListingsDto> AddItem(JobListingsDto itemDto)
        {
            // 1. הפיכת ה-DTO לישות (כאן עוברים ה-CategoryId וה-EmployerId)
            var jobEntity = mapper.Map<JobListings>(itemDto);

            // 2. שליחה לרפוסיטורי (שם כבר הגדרנו ש-Category ו-Employer יהיו null כדי למנוע שגיאות)
            var createdJob = await _repository.AddItem(jobEntity);

            // 3. החזרת התוצאה כ-DTO
            return mapper.Map<JobListingsDto>(createdJob);
        }

        public async Task DeleteItem(int id)
        {
            await _repository.DeleteItem(id);
        }

        public async Task<List<JobListingsDto>> GetAll()
        {
            return mapper.Map<List<JobListings>, List<JobListingsDto>>(await
                    _repository.GetAll());
        }
        public async Task<List<JobListingsDto>> GetJobByEmployer(int empId)
        {
            Console.WriteLine($"🔍 JobListingsService.GetJobByEmployer - Using extended repository for EmployerId: {empId}");

            // שימוש ב-Repository המורחב עם Include - מחזיר Entities
            //var jobsWithMatches = await _extendedRepository.GetJobByEmployerWithMatches(empId);
            var jobsEmployer = await _extendedRepository.GetJobByEmployer(empId);
            // מיפוי ה-Entities ל-DTOs עם AutoMapper
            return mapper.Map<List<JobListings>, List<JobListingsDto>>(jobsEmployer);
        }
        public async Task<JobListingsDto> GetById(int id)
        {
            return mapper.Map<JobListings, JobListingsDto>(await _repository.GetById(id));
        }

        public async Task<List<JobListings>> GetJobByEmployerWithMatches(int empId)
        {
            Console.WriteLine($"🔍 JobListingsService.GetJobByEmployerWithMatches - Using extended repository for EmployerId: {empId}");

            // שימוש ב-Repository המורחב עם Include - מחזיר Entities
            return await _extendedRepository.GetJobByEmployerWithMatches(empId);
        }

        public async Task<bool> ToggleJobStatus(int jobId, bool isActive)
        {
            // 1.שליפת המשרה הקיימת מה - Repository
            var job = await _repository.GetById(jobId);

            // 2. בדיקה אם המשרה קיימת
            if (job == null)
            {
                return false; // או לזרוק שגיאה לפי הסטנדרט שלכם
            }

            // 3. עדכון הסטטוס (שימוש ב-IsCatch כפי שהגדרת)
            job.IsCatch = isActive;

            // 4. שמירת השינויים ב-Repository
            // אני מניח שקיימת פונקציית Update או UpdateItem ב-Repository שלך
            await _repository.UpdateItem(jobId, job);

            return true; // החזרת הצלחה

        }

        public async Task UpdateItem(int id, JobListingsDto item)
        {
            var JobListingsEntity = mapper.Map<JobListingsDto, JobListings>(item);

            // 2. שולחים לרפוסיטורי את ה-ID ואת הישות הממופת
            await _repository.UpdateItem(id, JobListingsEntity);
        }


        // פונקציה 1: חישוב ציון בין מועמד למשרה
        public async Task<double> CalculateScore(int candidateUserId, int jobId)
        {
            // 1. שליפת המידע מה-DB
            var candidate = await _candidateRepository.GetById(candidateUserId);
            var job = await _repository.GetById(jobId);

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

            return score; // ציון מתוך 100
        }
        // פונקציה 2: החזרת 20 המשרות הכי מתאימות
        public async Task<List<JobSuggestionDto>> GetTopMatchesForCandidate(int candidateId)
        {
            // 1. שליפת כל המשרות הפעילות
            var allJobs = await _repository.GetAll();
            var activeJobs = allJobs.Where(j => !j.IsCatch).ToList();

            // שליפת כל המזהים של המשרות שהמועמד כבר הגיש אליהן - פעם אחת בלבד!
            var existingMatchJobIds = (await _matchService.GetAll())
                                        .Where(m => m.CandidateId == candidateId)
                                        .Select(m => m.JobId)
                                        .ToHashSet(); // HashSet לחיפוש מהיר

            var suggestions = new List<JobSuggestionDto>();

            foreach (var job in activeJobs)
            {
                // בדיקה מהירה בזיכרון במקום פנייה נוספת ל-DB
                if (!existingMatchJobIds.Contains(job.Id))
                {
                    double score = await CalculateScore(candidateId, job.Id);

                    // יצירת DTO שכולל את פרטי המשרה + הציון
                    var suggestion = new JobSuggestionDto
                    {
                        JobId = job.Id,
                        Title = job.Title,
                        Description = job.Description,
                        Location = job.Location,
                        Payment = job.Payment,
                        IsRemote = job.IsRemote,
                        MatchScore = score,
                        RequiredDate = job.RequiredDate
                    };
                    suggestions.Add(suggestion);
                }
            }

            // 3. מיון יורד לפי Score ולקיחת 20 הראשונים
            return suggestions
                .OrderByDescending(s => s.MatchScore)
                .Take(5)
                .ToList();
        }
        public async Task<List<EmployerOffersDto>> GetEmployerJobsDetailed(int empId)
        {
            // שליפת הנתונים מה-Repository
            var jobs = await _extendedRepository.GetJobsWithApplicants(empId);

            // מיפוי ידני או עם AutoMapper למבנה ה-DTO
            return jobs.Select(j => new EmployerOffersDto
            {
                JobId = j.Id,
                JobTitle = j.Title,
                TotalOffers = j.Matches.Count(m => m.Status == "Applied"), // רק אלו שהגישו הצעה
                Offers = j.Matches.Where(m => m.Status == "Applied").Select(m => new OfferDetailDto
                {
                    MatchId = m.Id,
                    CandidateName = m.Candidate.User.Name,
                    CandidateCity = m.Candidate.City,
                    MatchScore = m.MatchScore,
                    Status = m.Status
                }).ToList()
            }).ToList();
        }
        
    }
}
