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
        private readonly IRepository<Employer> _repository;
        private readonly IMapper mapper;
        public EmployerService(IRepository<Employer> repository, IMapper map)
        {
                _repository = repository;
                   mapper=map;
        }
        public async Task<List<JobListingsDto>> GetEmployerJobs(int employerId)
        {
            // משתמשים ב-_repository (עם קו תחתון) שהגדרת ב-Constructor
            var employer = await _repository.GetById(employerId);

            // אם המעסיק לא קיים או שרשימת המשרות ריקה
            if (employer == null || employer.MyJobs == null)
            {
                return new List<JobListingsDto>();
            }

            // משתמשים ב-mapper (שהגדרת כ-mapper) כדי להמיר את המשרות
            // אני מניח שיש לך פונקציית מיפוי ב-mapper או שאת משתמשת ב-mapper.Map
            var jobsDto = employer.MyJobs
                .Select(job => mapper.Map<JobListings, JobListingsDto>(job))
                .ToList();

            return jobsDto;
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
