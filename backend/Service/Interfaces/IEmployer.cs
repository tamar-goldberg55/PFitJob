using Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IEmployer:IService<EmployerDto>
    {
        Task<List<JobListingsDto>> GetEmployerJobs(int employerId);//שליפת כל המשרות שמעסיק מסוים פרסם 
        Task<EmployerDto> GetEmployerStats(int employerId);// שליפת נתונים סטטיסטים כמו משרות פעילות כמה התענינו 
    }
}
