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
        public CandidateService(IRepository<CandidateProfiles> repository, IMapper map)
        {
            _repository = repository;
            mapper = map;
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

        public Task<bool> UpdatePreferences(int candidateId, CandidateProfileDto preferences)
        {
            throw new NotImplementedException();
        }
    }
}
