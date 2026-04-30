using Repository.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IRepositoryCandidateProfiles : IRepository<CandidateProfiles>
    {
        Task<CandidateProfiles> GetByUserId(int userId);
    }
}
