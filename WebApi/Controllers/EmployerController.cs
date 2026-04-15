using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Dto;
using Service.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Authorize] // מחייב שיהיה טוקן כלשהו (גם מועמד וגם מעסיק)
    [Route("api/[controller]")]
    [ApiController]
    public class EmployerController : ControllerBase
    {
        private readonly IEmployer _employerService;

        // הזרקת השירות דרך ה-Constructor
        public EmployerController(IEmployer employerService)
        {
            _employerService = employerService;
        }

        // GET: api/Employer
        [HttpGet]
        public async Task<List<EmployerDto>> Get()
        {
            // מחזיר את הרשימה ישירות
            return await _employerService.GetAll();
        }

        // GET api/Employer/5
        [HttpGet("{id}")]
        public async Task<EmployerDto> Get(int id)
        {
            // מחזיר את האובייקט ישירות. 
            // אם לא נמצא, ה-Client יקבל בדרך כלל 204 No Content או null
            return await _employerService.GetById(id);
        }

        // GET api/Employer/5/jobs
        [HttpGet("{id}/jobs")]
        [Authorize(Roles = "Employer")]
        public async Task<List<JobListingsDto>> GetJobs(int id)
        {
            return await _employerService.GetEmployerJobs(id);
        }

        // POST api/Employer
        [HttpPost]
        public async Task<EmployerDto> Post([FromBody] EmployerDto employerDto)
        {
            return await _employerService.AddItem(employerDto);
        }

        // PUT api/Employer/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Employer")]
        public async Task Put(int id, [FromBody] EmployerDto employerDto)
        {
            await _employerService.UpdateItem(id, employerDto);
        }

        // DELETE api/Employer/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Employer")]
        public async Task Delete(int id)
        {
            await _employerService.DeleteItem(id);
        }
    }
}
