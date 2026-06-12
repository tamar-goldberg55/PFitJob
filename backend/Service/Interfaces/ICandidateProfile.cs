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
        Task<bool> UpdatePreferences(int candidateId, CandidateProfileDto preferences); //– עדכון הגדרות החיפוש.
        Task CandidateTakesJob(int candidateId, int jobId);//לבדוק את זה לאחר עדכון המשרות

    }
}
