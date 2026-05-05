using Microsoft.AspNetCore.Mvc;
using Service.Dto;
using Service.Interfaces;
using Repository.models;
using Microsoft.EntityFrameworkCore;
using CodeFirst;
using System.Security.Claims;
using AutoMapper;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly IMatch _matchService;
        private readonly DataBase _context;
        private readonly IMapper _mapper;

        public MatchController(IMatch matchService, DataBase context, IMapper mapper)
        {
            _matchService = matchService;
            _context = context;
            _mapper = mapper;
        }

        // פונקציית עזר למציאת ProfileId לפי UserId
        private async Task<int?> GetProfileIdByUserId(int userId)
        {
            var profile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            return profile?.Id;
        }

        // קבלת כל ההתאמות של המועמד המחובר
        [HttpGet]
        public async Task<ActionResult<List<MatchDto>>> Get()
        {
            Console.WriteLine("🚀 MatchController.Get - Starting request...");
            
            // חילוץ ה-ID של המשתמש מהטוקן
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                Console.WriteLine("❌ MatchController.Get - No userId found in token");
                return Unauthorized("חובה להתחבר מחדש כדי לצפות בנתונים.");
            }

            var userId = int.Parse(userIdString);
            Console.WriteLine($"🔍 MatchController.Get - UserId from token: {userId}");
            
            // מציאת ה-EmployerId של המשתמש
            var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserId == userId);
            if (employer == null)
            {
                Console.WriteLine($"❌ MatchController.Get - No employer found for UserId: {userId}");
                return Unauthorized("לא נמצא פרופיל מעסיק למשתמש זה.");
            }
            
            Console.WriteLine($"✅ MatchController.Get - Found EmployerId: {employer.Id} for UserId: {userId}");
            
            // שליפת כל המאצ'ים למשרות של המעסיק עם Include מלאים כדי למנוע Null
            var matches = await _context.Match
                .Include(m => m.Job) // צירוף פרטי המשרה
                .Include(m => m.Candidate) // צירוף פרטי המועמד
                .Include(m => m.Candidate.User) // צירוף פרטי המשתמש של המועמד
                .Where(m => m.Job.EmployerId == employer.Id)
                .ToListAsync();

            Console.WriteLine($"� MatchController.Get - Found {matches.Count} total matches for employer {employer.Id}");
            
            // לוגים מפורטים על המאצ'ים שנמצאו
            foreach (var match in matches)
            {
                Console.WriteLine($" Match ID: {match.Id}, Job: {match.Job?.Title}, Status: {match.Status}");
            }

            // וידוא שהמיפוי לא יכול להכשל
            try
            {
                var result = _mapper.Map<List<Match>, List<MatchDto>>(matches);
                Console.WriteLine($"✅ MatchController.Get - Successfully mapped {result.Count} matches to DTOs");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ MatchController.Get - Mapping error: {ex.Message}");
                return StatusCode(500, "שגיאה במיפוי הנתונים");
            }
        }

        // קבלת התאמה ספציפית
        [HttpGet("{id}")]
        public async Task<MatchDto> Get(int id)
        {
            return await _matchService.GetById(id);
        }

        // הרצת אלגוריתם השיבוץ האופטימלי (ה-DP)
        // POST: api/Match/run
        [HttpPost("run")]
        public async Task<List<MatchDto>> RunAlgorithm()
        {
            // העברתי 0 כברירת מחדל לפרמטר ה-dummy שהגדרת
            return await _matchService.RunMatchingAlgorithm(0);
        }

        // הרצת אלגוריתם השיבוץ רק עבור המועמד המחובר
        // POST: api/Match/run-for-me
        [HttpPost("run-for-me")]
        public async Task<ActionResult<List<MatchDto>>> RunAlgorithmForMe()
        {
            // חילוץ ה-ID של המשתמש מהטוקן
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("חובה להתחבר מחדש כדי להפעיל את האלגוריתם.");
            }

            var userId = int.Parse(userIdString);
            return await _matchService.RunMatchingAlgorithmForUser(userId);
        }

        // עדכון סטטוס מאץ'
        // PUT: api/Match/{id}/status
        [HttpPut("{id}/status")]
        public async Task<ActionResult<MatchDto>> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            // חילוץ ה-ID של המשתמש מהטוקן
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("חובה להתחבר מחדש כדי לעדכן סטטוס.");
            }

            var userId = int.Parse(userIdString);

            // מציאת המאץ' ובדיקה שהוא שייך למשתמש המחובר
            var match = await _context.Match
                .Include(m => m.Candidate)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (match == null)
            {
                return NotFound("מאץ' לא נמצא.");
            }

            if (match.Candidate.UserId != userId)
            {
                return Unauthorized("אין לך הרשאות לעדכן מאץ' זה.");
            }

            // עדכון הסטטוס
            match.Status = request.Status;
            await _context.SaveChangesAsync();

            return _mapper.Map<Match, MatchDto>(match);
        }

        // קבלת ההתאמה הכי טובה עבור מועמד ספציפי (לפי האלגוריתם)
        // GET: api/Match/candidate/5
        [HttpGet("candidate/{candidateId}")]
        public async Task<List<MatchDto>> GetTopMatches(int candidateId, [FromQuery] int topCount = 1)
        {
            return await _matchService.GetTopMatchesForCandidate(candidateId, topCount);
        }

        // קבלת מועמדים שאישרו משרה ספציפית
        // GET: api/Match/job/{jobId}/accepted-candidates
        [HttpGet("job/{jobId}/accepted-candidates")]
        public async Task<ActionResult<List<MatchDto>>> GetAcceptedCandidatesForJob(int jobId)
        {
            // חילוץ ה-ID של המשתמש מהטוקן
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("חובה להתחבר מחדש כדי לצפות בנתונים.");
            }

            var userId = int.Parse(userIdString);
            
            // שליפת המאצ'ים המאושרים למשרה הספציפית של המעסיק
            var acceptedMatches = await _context.Match
                .Include(m => m.Candidate)
                .Include(m => m.Job)
                .Where(m => m.JobId == jobId && 
                          m.Job.EmployerId == userId && // וידוא שהמשרה שייכת למעסיק המחובר
                          m.Status == "accepted")
                .ToListAsync();

            return _mapper.Map<List<Match>, List<MatchDto>>(acceptedMatches);
        }

        // קבלת מדד שביעות רצון כללי של המערכת
        // GET: api/Match/satisfaction
        [HttpGet("satisfaction")]
        public async Task<ActionResult<double>> GetSatisfactionScore()
        {
            return await _matchService.GetSatisfactionScore();

        }

        // חישוב ציון התאמה תיאורטי בין מועמד למשרה
        // GET: api/Match/score?candidateId=1&jobId=2
        [HttpGet("score")]
        public async Task<double> GetMatchScore([FromQuery] int candidateId, [FromQuery] int jobId)
        {
            return await _matchService.CalculateMatchScore(candidateId, jobId);
        }

        // שליחת הצעה למועמד
        [HttpPost("send-offer")]

        public async Task<ActionResult<MatchDto>> SendOffer([FromBody] SendOfferRequest request)
        {
            // משיכת ה-ID של המשתמש המחובר מהטוקן
            //var userIdString = User.Identity.NameIdentifier;
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("חובה להתחבר מחדש כדי לצפות בנתונים.");
            }

            var userId = int.Parse(userIdString);
            
            // מציאת ה-ProfileId לפי ה-UserId
            var profileId = await GetProfileIdByUserId(userId);
            if (profileId == null)
            {
                throw new NotFoundException("Candidate profile not found for user");
            }

            // יצירת מאץ' עם ה-ID של הפרופיל
            var matchDto = new MatchDto
            {
                CandidateId = profileId.Value, // ה-ID של הפרופיל
                JobId = request.JobId,
                MatchScore = 0, // יחושב מאוחר יותר
                Status = "pending",
                MatchDate = DateTime.Now,
                IsSelectedByAlgorithm = false
            };

            return await _matchService.AddItem(matchDto);
        }

        // הוספת התאמה ידנית
        [HttpPost]
        public async Task<MatchDto> Post([FromBody] MatchDto matchDto)
        {
            return await _matchService.AddItem(matchDto);
        }

        // עדכון התאמה
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody] MatchDto matchDto)
        {
            await _matchService.UpdateItem(id, matchDto);
        }

        // מחיקת התאמה
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await _matchService.DeleteItem(id);
        }
    }

    // מחלקה לבקשת עדכון סטטוס
    public class UpdateStatusRequest
    {
        public string Status { get; set; }
    }
}
