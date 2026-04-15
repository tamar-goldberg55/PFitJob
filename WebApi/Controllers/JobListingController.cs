using Microsoft.AspNetCore.Authorization; // חובה להוסיף
using Microsoft.AspNetCore.Mvc;
using Service.Dto;
using Service.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobListingController : ControllerBase
    {
        private readonly IJobListings _jobListingsService;

        public JobListingController(IJobListings jobListingsService)
        {
            _jobListingsService = jobListingsService;
        }

        // GET: api/JobListings
        // פתוח לכולם - אנחנו רוצים שגם מי שלא מחובר יוכל לראות משרות
        [HttpGet]
        public async Task<List<JobListingsDto>> Get()
        {
            return await _jobListingsService.GetAll();
        }

        // GET api/JobListings/5
        // פתוח לכולם
        [HttpGet("{id}")]
        public async Task<JobListingsDto> Get(int id)
        {
            return await _jobListingsService.GetById(id);
        }

        // POST api/JobListings
        // רק מעסיק יכול לפרסם משרה חדשה
        [Authorize(Roles = "Employer")]
        [HttpPost]
        public async Task<JobListingsDto> Post([FromBody] JobListingsDto jobDto)
        {
            return await _jobListingsService.AddItem(jobDto);
        }

        // PUT api/JobListings/5
        // רק מעסיק יכול לעדכן משרה
        [Authorize(Roles = "Employer")]
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody] JobListingsDto jobDto)
        {
            await _jobListingsService.UpdateItem(id, jobDto);
        }

        // DELETE api/JobListings/5
        // רק מעסיק יכול למחוק משרה
        [Authorize(Roles = "Employer")]
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await _jobListingsService.DeleteItem(id);
        }

        // רק מעסיק יכול לשנות סטטוס משרה
        [Authorize(Roles = "Employer")]
        [HttpPatch("{id}/status")]
        public async Task<bool> ToggleStatus(int id, [FromQuery] bool isActive)
        {
            return await _jobListingsService.ToggleJobStatus(id, isActive);
        }
    }
}