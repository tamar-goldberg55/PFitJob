using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Interfaces;
using Repository.models;
using Service.Dto;
using Service.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Authorize] // רק מי שיש לו טוקן תקין יכול להיכנס (ללא בדיקת Role)
    [Route("api/Candidate")]
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
        public async Task<List<CandidateProfileDto>> GetAll()
        {
            // חייב await כאן כדי שה-Task יהפוך לרשימה אמיתית
            return await _candidateService.GetAll();
        }

        // קבלת מועמד לפי ID
        [HttpGet("{id}")]
        public Task<CandidateProfileDto> Get(int id)
        {
            return _candidateService.GetById(id);
        }

        // יצירת מועמד חדש
        [HttpPost("profile")]
        public async Task<ActionResult<CandidateProfileDto>> Post([FromBody] CandidateProfileDto candidateDto)
        {
            // חילוץ ה-ID של המשתמש מה-Claims של הטוקן - ננסה כמה שמות אפשריים
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("UserId")?.Value
                              ?? User.FindFirst("id")?.Value
                              ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                // לצורך דיבאג - נדפיס את כל ה-claims שקיימים
                var allClaims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
                return BadRequest($"לא נמצא מזהה משתמש בטוקן. Claims קיימים: {string.Join(", ", allClaims)}");
            }

            // המרה של ה-ID למספר (int)
            if (!int.TryParse(userIdClaim, out int candidateId))
            {
                return BadRequest("מזהה משתמש לא תקין");
            }

            // הגדרת ה-ID של המשתמש ב-DTO
            candidateDto.Id = candidateId;

            try
            {
                var result = await _candidateService.AddItem(candidateDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"שגיאה ביצירת פרופיל: {ex.Message}");
            }
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

        [Authorize] // חשוב מאוד: מוודא שרק מי שיש לו טוקן יכול להיכנס
        [HttpGet("my-profile")]
        public async Task<ActionResult<CandidateProfileDto>> GetMyProfile()
        {
            // 1. חילוץ ה-ID של המשתמש מה-Claims של הטוקן - ננסה כמה שמות אפשריים
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("UserId")?.Value
                              ?? User.FindFirst("id")?.Value
                              ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                // לצורך דיבאג - נדפיס את כל ה-claims שקיימים
                var allClaims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
                return BadRequest($"לא נמצא מזהה משתמש בטוקן. Claims קיימים: {string.Join(", ", allClaims)}");
            }

            // 2. המרה של ה-ID למספר (int)
            if (!int.TryParse(userIdClaim, out int candidateId))
            {
                return BadRequest("מזהה משתמש לא תקין");
            }

            // 3. שליפת הפרופיל האמיתי מהשירות
            var profile = await _candidateService.GetById(candidateId);

            if (profile == null) return NotFound("לא נמצא פרופיל למשתמש המחובר");

            return Ok(profile);
        }
    }
}
