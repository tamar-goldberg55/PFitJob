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
    public class CandidateService:ICandidateProfile
    {
        private readonly IRepository<CandidateProfiles> _repository;
        private readonly IMapper mapper;
        private readonly IJobListings _jobService; // הוספנו משתנה חדש
        public CandidateService(IRepository<CandidateProfiles> repository, IMapper map, IJobListings jobService)
        {
            _repository = repository;
            mapper = map;
            _jobService = jobService; // שמירה במשתנה מחלקתי
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

        public Task<List<CandidateProfileDto>> GetAll()
        {
            return mapper.Map<Task<List<CandidateProfiles>>, Task<List<CandidateProfileDto>>>(_repository.GetAll());
        }

        public async Task<CandidateProfileDto> GetById(int id)
        {
              return mapper.Map<CandidateProfiles, CandidateProfileDto>(await _repository.GetById(id));
        }

        public Task<JobListingsDto> GetMatchingJobs(int candidateId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateItem(int id, CandidateProfileDto item)
        {
            throw new NotImplementedException();
        }

      
        public async Task<bool> UpdatePreferences(int candidateId, CandidateProfileDto preferences)
        {
            // 1. שליפת המועמד הקיים
            CandidateProfiles can = await _repository.GetById(candidateId);

            if (can == null) return false;

            // 2. עדכון השדות (שימי לב שאני לא נוגעת ב-Id וב-User)
            can.activity = preferences.Activity;
            can.City = preferences.City;
            can.MaxDistance = preferences.MaxDistance;
            can.IsRemoteOnly = preferences.IsRemoteOnly;
            can.level = preferences.Level;
            can.MinHourlyRate = preferences.MinHourlyRate;
            can.Withpepole = preferences.WithPeople;

            // 3. עדכון בבסיס הנתונים (שימוש ב-UpdateItem שהגדרנו)
            await _repository.UpdateItem(candidateId, can);

            return true;
        }
    }
}
