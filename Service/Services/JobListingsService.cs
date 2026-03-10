using AutoMapper;
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
        private readonly IMapper mapper;
        public JobListingsService(IRepository<JobListings> repository, IMapper map)
        {
            _repository = repository;
            mapper= map;    
                
        }
        public async Task<JobListingsDto> AddItem(JobListingsDto item)
        {
            return mapper.Map<JobListings, JobListingsDto>(

            await _repository.AddItem(mapper.Map<JobListingsDto, JobListings>(item)));
        }

        public  async Task DeleteItem(int id)
        {
            await _repository.DeleteItem(id);
        }

        public async Task<List<JobListingsDto>> GetAll()
        {
            return mapper.Map<List<JobListings>, List<JobListingsDto>>(await
                    _repository.GetAll());
        }

        public async Task<JobListingsDto> GetById(int id)
        {
            return mapper.Map<JobListings, JobListingsDto>(await _repository.GetById(id));
        }

        public Task<bool> ToggleJobStatus(int jobId, bool isActive)
        {
            throw new NotImplementedException();
        }

        public Task UpdateItem(int id, JobListingsDto item)
        {
            throw new NotImplementedException();
        }
    }
}
