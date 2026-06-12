using Repository.models;
using Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface ICandidateProfile:IService<CandidateProfileDto>
    {
        Task<JobListingsDto> GetMatchingJobs(int candidateId);//מחזירה את המשרה הכי תואמת
        Task<CandidateProfiles> UpdatePreferences(int candidateId, CandidateProfileDto preferences); // Upsert: עדכון או יצירת פרופיל - מחזיר אובייקט
        Task CandidateTakesJob(int candidateId, int jobId);//לבדוק את זה לאחר עדכון המשרות
        Task<CandidateProfileDto> GetByUserId(int userId); // חיפוש פרופיל לפי UserId
        Task<CandidateProfileDto> GetTOEmp(int userId);

    }
}
