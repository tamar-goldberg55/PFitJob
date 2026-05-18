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
    public class EmployerService:IEmployer
    {
        private readonly IRepositoryEmployer _repository;
        private readonly IMapper mapper;
        public EmployerService(IRepositoryEmployer repository, IMapper map)
        {
                _repository = repository;
                   mapper=map;
        }
      
        public async Task<List<JobListingsDto>> GetEmployerJobs(int userId)
        {
            var employer = await _repository.GetByUserId(userId);
            if (employer == null || employer.MyJobs == null)
            {
                return new List<JobListingsDto>();
            }
            return mapper.Map<List<JobListingsDto>>(employer.MyJobs);
        }
        public Task<EmployerDto> GetEmployerStats(int employerId)
        {
            throw new NotImplementedException();
        }


        public async Task<List<EmployerDto>> GetAll()
        {
            var employers = await _repository.GetAll(); // מחכים לנתונים מה-DB
            return mapper.Map<List<EmployerDto>>(employers); // ממפים את הרשימה האמיתית
        }
        public async Task<EmployerDto> GetById(int id)
        {
            return mapper.Map<Employer, EmployerDto>(await _repository.GetById(id));
        }
        public async Task<EmployerDto> GetEmployerByUserId(int userId)
        {
            return mapper.Map<Employer, EmployerDto>(await _repository.GetByUserId(userId));

        }
        public async Task<EmployerDto> AddItem(EmployerDto item)
        {
            return mapper.Map<Employer, EmployerDto>(
          await _repository.AddItem(mapper.Map<EmployerDto, Employer>(item)));
        }

        public async Task UpdateItem(int id, EmployerDto item)
        {
            var EmployerEntity = mapper.Map<EmployerDto, Employer>(item);

            // 2. שולחים לרפוסיטורי את ה-ID ואת הישות הממופת
            await _repository.UpdateItem(id, EmployerEntity);
        }

        public async Task DeleteItem(int id)
        {
            await _repository.DeleteItem(id);
        }
    }
}
