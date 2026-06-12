using Repository.models;
using Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IJobListings : IService<JobListingsDto>
    {
        Task<bool> ToggleJobStatus(int jobId, bool isActive);//פתיחה או סגירה של משרה 
        Task<List<JobListingsDto>> GetJobByEmployer(int empId);
        Task<List<JobListings>> GetJobByEmployerWithMatches(int empId); // מתודה חדשה עם Include למאצ'ים - מחזירה Entities
        Task<double> CalculateScore(int candidateId, int jobId);
        Task<List<JobSuggestionDto>> GetTopMatchesForCandidate(int candidateId);
        Task<List<EmployerOffersDto>> GetEmployerJobsDetailed(int empId);

    }
}
