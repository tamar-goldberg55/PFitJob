using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Dto;
using Service.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
 // מחייב שיהיה טוקן כלשהו (גם מועמד וגם מעסיק)
    [Route("api/Employer")]
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
        //[HttpGet("{id}/jobs")]
        //[Authorize(Roles = "Employer")]
        //public async Task<List<JobListingsDto>> GetJobs(int id)
        //{

        //    return await _employerService.GetEmployerJobs(id);
        //}
        // שנה מ-AllowAnonymous ל-Authorize כדי שהטוקן ייקרא
        //[HttpGet("{id}/jobs")]
        //public async Task<ActionResult> GetJobs(int id)
        //{
        //    Console.WriteLine($"DEBUG: Received ID from frontend is: {id}");
        //    // 1. חילוץ ה-ID מהטוקן
        //    var userIdFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        //    // 2. בדיקה לוגית - האם זה המשתמש הנכון?
        //    if (userIdFromToken != id.ToString())
        //    {
        //        // מחזיר סטטוס 403 (Forbidden) עם ההודעה שלך כטקסט
        //        return StatusCode(403, "אין לך הרשאה לצפות במשרות של מעסיק אחר");
        //    }

        //    var jobs = await _employerService.GetEmployerJobs(id);
        //    return Ok(jobs);
        //}
        [HttpGet("{id}/jobs")]
        public async Task<ActionResult> GetJobs(int id)
        {
            try
            {
                var userIdFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userIdFromToken != id.ToString())
                {
                    return StatusCode(403, "אין לך הרשאה לצ במשרות של מעסיק אחר");
                }
                var jobs = await _employerService.GetEmployerJobs(id);
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"שגיאה בטעינת משרות: {ex.Message}");
            }
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
