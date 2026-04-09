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
        //public async Task<JobListingsDto> AddItem(JobListingsDto item)
        //{
        //    return mapper.Map<JobListings, JobListingsDto>(

        //    await _repository.AddItem(mapper.Map<JobListingsDto, JobListings>(item)));
        //}
        public async Task<JobListingsDto> AddItem(JobListingsDto itemDto)
        {
            // 1. הפיכת ה-DTO לישות (כאן עוברים ה-CategoryId וה-EmployerId)
            var jobEntity = mapper.Map<JobListings>(itemDto);

            // 2. שליחה לרפוסיטורי (שם כבר הגדרנו ש-Category ו-Employer יהיו null כדי למנוע שגיאות)
            var createdJob = await _repository.AddItem(jobEntity);

            // 3. החזרת התוצאה כ-DTO
            return mapper.Map<JobListingsDto>(createdJob);
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
            await _repository.UpdateItem(jobId,job);

            return true; // החזרת הצלחה

        }

        public async Task UpdateItem(int id, JobListingsDto item)
        {
            var JobListingsEntity = mapper.Map<JobListingsDto, JobListings>(item);

            // 2. שולחים לרפוסיטורי את ה-ID ואת הישות הממופת
            await _repository.UpdateItem(id, JobListingsEntity);
        }
    }
}
