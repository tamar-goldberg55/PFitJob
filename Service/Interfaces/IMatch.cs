using Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IMatch : IService<MatchDto>
    {
        
        Task<MatchDto> GetMatchByJC(int idJob, int idCandidate);
        Task<List<MatchDto>> GetMatchsByEmpID(int idEmp);
        Task<List<MatchDto>> GetRejecteds(int idEmp);
        Task<bool> ApplyForJob(int candidateId, int jobId);
        Task<double> CalculateScorEmp(int candidateId, int jobId);
        Task<MatchDto> MostMatch(int jobId);

    }
}

