using AutoMapper;
using Repository.models;
using Service.Dto;
using Service.Interfaces;
using Repository.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class CandidateService : ICandidateProfile
    {
        private readonly IRepositoryCandidateProfiles _repository;
        private readonly IMapper mapper;
        private readonly IJobListings _jobService; // הוספנו משתנה חדש
        private readonly IMatch _matchService; // הזרקה של שירות השידוכים
        public CandidateService(IRepositoryCandidateProfiles repository, IMapper map, IJobListings jobService, IMatch matchService)
        {
            _repository = repository;
            mapper = map;
            _jobService = jobService; // שמירה במשתנה מחלקתי
            _matchService = matchService;
        }
        public async Task CandidateTakesJob(int candidateId, int jobId)
        {
            // 1. עדכון המשרה דרך ה-Service של המשרות
            await _jobService.ToggleJobStatus(jobId, false);

            // 2. עדכון המועמד דרך ה-Repository המקומי
            var candidate = await _repository.GetById(candidateId);
            if (candidate != null)
            {
                candidate.activity = false;
                // אנחנו מעבירים את ה-ID ואת האובייקט המעודכן ל-UpdateItem
                await _repository.UpdateItem(candidateId, candidate);
            }
        }
        public async Task<CandidateProfileDto> AddItem(CandidateProfileDto item)
        {
            return mapper.Map<CandidateProfiles, CandidateProfileDto>(
           await _repository.AddItem(mapper.Map<CandidateProfileDto, CandidateProfiles>(item)));
        }

        public async Task DeleteItem(int id)
        {
            await _repository.DeleteItem(id);
        }

        public async Task<List<CandidateProfileDto>> GetAll()
        {
            // 1. מחכים שה-Repository יסיים להביא את הנתונים מה-DB
            var candidates = await _repository.GetAll();

            // 2. ממפים את הרשימה שחזרה (List) לרשימה של ה-Dto
            return mapper.Map<List<CandidateProfileDto>>(candidates);
        }

        public async Task<CandidateProfileDto> GetById(int id)
        {
              return mapper.Map<CandidateProfiles, CandidateProfileDto>(await _repository.GetById(id));
        }

        public async Task<CandidateProfileDto> GetByUserId(int userId)
        {
            var profile = await _repository.GetByUserId(userId);
            if (profile == null) return null;
            return mapper.Map<CandidateProfiles, CandidateProfileDto>(profile);
        }

        public async Task<JobListingsDto> GetMatchingJobs(int candidateId)
        {
            // אנחנו קוראים לפונקציה שבנינו קודם ב-MatchService
            // היא כבר יודעת להסתכל על IsSelectedByAlgorithm
            //var topMatches = await _matchService.GetTopMatchesForCandidate(candidateId, 1);

            //var bestMatch = topMatches.FirstOrDefault();

            //if (bestMatch == null) return null;

            //// מחזירים את אובייקט המשרה מתוך ה-Match
            //// הערה: ודאי שב-MatchDto שלך יש שדה שנקרא Job מסוג JobListingsDto
            //return mapper.Map<JobListingsDto>(bestMatch.Job);
            return null;
        }

        public async Task UpdateItem(int id, CandidateProfileDto item)
        {
            var CandidateProfileEntity = mapper.Map<CandidateProfileDto, CandidateProfiles>(item);

            // 2. שולחים לרפוסיטורי את ה-ID ואת הישות הממופת
            await _repository.UpdateItem(id, CandidateProfileEntity);
        }

      public async Task<CandidateProfileDto> GetTOEmp(int Id)
        {
            var profile = await _repository.GetById(Id);
            if (profile == null) return null;
            return mapper.Map<CandidateProfiles, CandidateProfileDto>(profile);
        }
        public async Task<CandidateProfiles> UpdatePreferences(int candidateId, CandidateProfileDto preferences)
        {
            Console.WriteLine($"DEBUG: UpdatePreferences (Upsert) called with candidateId={candidateId}");
            
            // 1. שליפת המועמד לפי UserId
            CandidateProfiles existingProfile = await _repository.GetByUserId(candidateId);
            
            Console.WriteLine($"DEBUG: GetByUserId returned: {(existingProfile == null ? "NULL" : $"ProfileId={existingProfile.Id}, UserId={existingProfile.UserId}")}");

            if (existingProfile != null)
            {
                // 2. עדכון הפרופיל הקיים
                existingProfile.activity = preferences.Activity;
                existingProfile.City = preferences.City;
                existingProfile.MaxDistance = preferences.MaxDistance;
                existingProfile.IsRemoteOnly = preferences.IsRemoteOnly;
                existingProfile.level = preferences.Level;
                existingProfile.MinHourlyRate = preferences.MinHourlyRate;
                existingProfile.Withpepole = preferences.WithPeople;
                existingProfile.CategoryId = preferences.CategoryId;

                // 3. עדכון בבסיס הנתונים
                await _repository.UpdateItem(existingProfile.Id, existingProfile);
                Console.WriteLine($"DEBUG: Updated existing profile {existingProfile.Id}");
                return existingProfile; // החזרת את הפרופיל שעודכן
            }
            else
            {
                // 4. יצירת פרופיל חדש אם לא קיים
                var newProfile = new CandidateProfiles
                {
                    UserId = candidateId, // ה-UserId הוא ה-ID של המשתמש המאומת
                    activity = preferences.Activity,
                    City = preferences.City,
                    MaxDistance = preferences.MaxDistance,
                    IsRemoteOnly = preferences.IsRemoteOnly,
                    level = preferences.Level,
                    MinHourlyRate = preferences.MinHourlyRate,
                    Withpepole = preferences.WithPeople,
                    CategoryId = preferences.CategoryId
                };

                var createdProfile = await _repository.AddItem(newProfile);
                Console.WriteLine($"DEBUG: Created new profile {createdProfile.Id} for user {candidateId}");
                return createdProfile; // החזרת את הפרופיל שנוצר



            }
            
        }
    }
}
