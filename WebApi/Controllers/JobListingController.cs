using Microsoft.AspNetCore.Mvc;
using Service.Dto;
using Service.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobListingController : ControllerBase
    {
        private readonly IJobListings _jobListingsService;

        // הזרקת השירות (Dependency Injection)
        public JobListingController(IJobListings jobListingsService)
        {
            _jobListingsService = jobListingsService;
        }

        // GET: api/JobListings
        [HttpGet]
        public async Task<List<JobListingsDto>> Get()
        {
            return await _jobListingsService.GetAll();
        }

        // GET api/JobListings/5
        [HttpGet("{id}")]
        public async Task<JobListingsDto> Get(int id)
        {
            return await _jobListingsService.GetById(id);
        }

        // POST api/JobListings
        [HttpPost]
        public async Task<JobListingsDto> Post([FromBody] JobListingsDto jobDto)
        {
            return await _jobListingsService.AddItem(jobDto);
        }

        // PUT api/JobListings/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody] JobListingsDto jobDto)
        {
            await _jobListingsService.UpdateItem(id, jobDto);
        }

        // DELETE api/JobListings/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await _jobListingsService.DeleteItem(id);
        }

        // בונוס: פונקציה לעדכון סטטוס המשרה (Toggle) כפי שהגדרת בסרביס
        [HttpPatch("{id}/status")]
        public async Task<bool> ToggleStatus(int id, [FromQuery] bool isActive)
        {
            return await _jobListingsService.ToggleJobStatus(id, isActive);
        }
    }
}

