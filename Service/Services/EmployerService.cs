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
    public class EmployerService:IEmployer
    {
        private readonly IRepository<Employer> _repository;
        private readonly IMapper mapper;
        public EmployerService(IRepository<Employer> repository, IMapper map)
        {
                _repository = repository;
                   mapper=map;
        }
        public Task<List<JobListingsDto>> GetEmployerJobs(int employerId)
        {
            throw new NotImplementedException();
        }

        public Task<EmployerDto> GetEmployerStats(int employerId)
        {
            throw new NotImplementedException();
        }

        public Task<List<EmployerDto>> GetAll()
        {
            return mapper.Map<Task<List<Employer>>, Task<List<EmployerDto>>>(_repository.GetAll());
        }

        public async Task<EmployerDto> GetById(int id)
        {
            return mapper.Map<Employer, EmployerDto>(await _repository.GetById(id));
        }

        public async Task<EmployerDto> AddItem(EmployerDto item)
        {
            return mapper.Map<Employer, EmployerDto>(
          await _repository.AddItem(mapper.Map<EmployerDto, Employer>(item)));
        }

        public Task UpdateItem(int id, EmployerDto item)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteItem(int id)
        {
            await _repository.DeleteItem(id);
        }
    }
}
