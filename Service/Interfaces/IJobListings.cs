using Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IJobListings:IService<JobListingsDto>
    {
        Task<bool> ToggleJobStatus(int jobId, bool isActive);//פתיחה או סגירה של משרה 
    }
}
