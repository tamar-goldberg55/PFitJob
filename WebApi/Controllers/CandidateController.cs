using Microsoft.AspNetCore.Mvc;
using Repository.Interfaces;
using Repository.models;
using Service.Dto;
using Service.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateController : ControllerBase
    {
        private readonly ICandidateProfile _candidateService;

        // הזרקה של ה-Service במקום ה-Repository
        public CandidateController(ICandidateProfile candidateService)
        {
            _candidateService = candidateService;
        }

        // קבלת כל המועמדים
        [HttpGet]
        public Task<List<CandidateProfileDto>> GetAll()
        {
            return _candidateService.GetAll();
        }

        // קבלת מועמד לפי ID
        [HttpGet("{id}")]
        public Task<CandidateProfileDto> Get(int id)
        {
            return _candidateService.GetById(id);
        }

        // יצירת מועמד חדש
        [HttpPost]
        public Task<CandidateProfileDto> Post([FromBody] CandidateProfileDto candidateDto)
        {
            return _candidateService.AddItem(candidateDto);
        }

        // עדכון פרטי מועמד
        [HttpPut("{id}")]
        public Task Update(int id, [FromBody] CandidateProfileDto candidateDto)
        {
            return _candidateService.UpdateItem(id, candidateDto);
        }

        // מחיקת מועמד
        [HttpDelete("{id}")]
        public Task Delete(int id)
        {
            return _candidateService.DeleteItem(id);
        }

        // פונקציה מיוחדת: עדכון העדפות מועמד
        [HttpPatch("{id}/preferences")]
        public Task<bool> UpdatePreferences(int id, [FromBody] CandidateProfileDto preferences)
        {
            return _candidateService.UpdatePreferences(id, preferences);
        }

        // פונקציה מיוחדת: מציאת המשרה הכי מתאימה
        [HttpGet("{id}/best-match")]
        public Task<JobListingsDto> GetBestMatch(int id)
        {
            return _candidateService.GetMatchingJobs(id);
        }

        // פונקציה מיוחדת: מועמד מאשר לקיחת משרה
        [HttpPost("{id}/take-job/{jobId}")]
        public Task TakeJob(int id, int jobId)
        {
            return _candidateService.CandidateTakesJob(id, jobId);
        }
    }
}
