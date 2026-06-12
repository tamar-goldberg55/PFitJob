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


        [HttpGet("getByEmp/{empId}")]
        public async Task<List<MatchDto>> GetByEmp(int empId)
        {
            return await _matchService.GetMatchsByEmpID(empId);
        }

        [HttpGet("getRejecteds/{empId}")]
        public async Task<List<MatchDto>> GetRejecteds(int empId)
        {
            return await _matchService.GetRejecteds(empId);
        }
        [HttpGet("mostMatch/{jobId}")]
        public async Task<ActionResult<MatchDto>> MostMatch(int jobId)
        {
            var match = await _matchService.MostMatch(jobId);
            if (match == null)
            {
                return NotFound("No match found for the given job ID.");
            }
            return Ok(match);
        }



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
        //[HttpGet("candidate/{candidateId}")]
        //public async Task<List<MatchDto>> GetTopMatches(int candidateId, [FromQuery] int topCount = 1)
        //{
        //    return await _matchService.GetTopMatchesForCandidate(candidateId, topCount);
        //}

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
        //[HttpGet("satisfaction")]
        //public async Task<ActionResult<double>> GetSatisfactionScore()
        //{
        //    return await _matchService.GetSatisfactionScore();

        //}

        // חישוב ציון התאמה תיאורטי בין מועמד למשרה
        // GET: api/Match/score?candidateId=1&jobId=2
        //[HttpGet("score")]
        //public async Task<double> GetMatchScore([FromQuery] int candidateId, [FromQuery] int jobId)
        //{
        //    return await _matchService.CalculateMatchScore(candidateId, jobId);
        //}

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

        // קבלת הצעות למועמד - GET: api/Match/candidate-suggestions
        //[HttpGet("candidate-suggestions")]
        //public async Task<ActionResult<List<JobSuggestionDto>>> GetCandidateSuggestions()
        //{
        //    Console.WriteLine("🚀 MatchController.GetCandidateSuggestions - Starting request...");

        //    // חילוץ ה-ID של המשתמש מהטוקן
        //    var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(userIdString))
        //    {
        //        Console.WriteLine("❌ MatchController.GetCandidateSuggestions - No userId found in token");
        //        return Unauthorized("חובה להתחבר מחדש כדי לצפות בהצעות.");
        //    }

        //    var userId = int.Parse(userIdString);
        //    Console.WriteLine($"🔍 MatchController.GetCandidateSuggestions - UserId from token: {userId}");

        //    // מציאת ה-CandidateProfile לפי UserId
        //    var profileId = await GetProfileIdByUserId(userId);
        //    if (profileId == null)
        //    {
        //        Console.WriteLine($"❌ MatchController.GetCandidateSuggestions - No profile found for UserId: {userId}");
        //        return NotFound("לא נמצא פרופיל מועמד.");
        //    }

        //    // שליפת כל המשרות הפעילות
        //    var activeJobs = await _context.JobListings
        //        .Where(j => j.IsCatch == true)
        //        .ToListAsync();

        //    Console.WriteLine($"📋 MatchController.GetCandidateSuggestions - Found {activeJobs.Count} active jobs");

        //    var suggestions = new List<JobSuggestionDto>();

        //    // הרצת CalculateMatchScore על כל המשרות הפעילות
        //    foreach (var job in activeJobs)
        //    {
        //        var score = await _matchService.CalculateMatchScore(profileId.Value, job.Id);

        //        // החזר רק משרות עם ציון > 0
        //        if (score > 0)
        //        {

        //            var levelLabel = ((int)job.leveJob) switch
        //            {
        //                0 => "קלה",
        //                1 => "בינונית",
        //                2 => "קשה", // או מה שהערך 2 מייצג אצלך
        //                _ => "לא ידוע" // תמיד כדאי להוסיף מקרה ברירת מחדל ב-switch
        //            };

        //            suggestions.Add(new JobSuggestionDto
        //            {
        //                JobId = job.Id,
        //                Title = job.Title,
        //                Description = job.Description,
        //                Location = job.Location,
        //                Payment = job.Payment,
        //                IsRemote = job.IsRemote,
        //                IsJobWithPeople = job.IsJobWithPepole,
        //                LevelLabel = levelLabel,
        //                MatchScore = score,
        //                RequiredDate = job.RequiredDate,
        //                EmployerId = job.EmployerId
        //            });
        //        }
        //    }

        //    // מיון מהגבוה לנמוך לפי הציון
        //    var sortedSuggestions = suggestions.OrderByDescending(s => s.MatchScore).ToList();

        //    Console.WriteLine($"✅ MatchController.GetCandidateSuggestions - Returning {sortedSuggestions.Count} suggestions");
        //    return sortedSuggestions;
        //}

        // הגשת מועמד למשרה - POST: api/Match/apply
        [HttpPost("apply")]
        public async Task<ActionResult<MatchDto>> ApplyToJob([FromBody] ApplyRequest request)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

                var userId = int.Parse(userIdString);
                var profileId = await GetProfileIdByUserId(userId);
                if (profileId == null) return NotFound("Candidate profile not found");

                // קריאה ל-Service במקום לכתוב את הלוגיקה כאן
                var result = await _matchService.ApplyForJob(profileId.Value, request.JobId);

                // החזרת DTO מתאים
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ MatchController.ApplyToJob - Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

       

        // קבלת הצעות למעסיק - GET: api/Match/employer-offers
        [HttpGet("employer-offers")]
        public async Task<ActionResult<List<EmployerOffersDto>>> GetEmployerOffers()
        {
            Console.WriteLine("🚀 MatchController.GetEmployerOffers - Starting request...");

            // חילוץ ה-ID של המשתמש מהטוקן
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("חובה להתחבר מחדש כדי לצפות בהצעות.");
            }

            var userId = int.Parse(userIdString);

            // שליפת ה-EmployerId לפי UserId
            var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserId == userId);
            if (employer == null)
            {
                return NotFound("לא נמצא פרופיל מעסיק.");
            }

            // שליפת כל ה-Matches למשרות של המעסיק
            var matches = await _context.Match
                .Include(m => m.Job)
                .Include(m => m.Candidate)
                .Include(m => m.Candidate.User)
                .Where(m => m.Job.EmployerId == employer.Id)
                .ToListAsync();

            Console.WriteLine($"📋 MatchController.GetEmployerOffers - Found {matches.Count} total matches");

            // מיון לפי JobId וקיבוץ מקובץ
            var groupedMatches = matches
                .GroupBy(m => m.JobId)
                .Select(g => new EmployerOffersDto
                {
                    JobId = g.Key,
                    JobTitle = g.First().Job.Title,
                    TotalOffers = g.Count(),
                    Offers = g.Select(m => new OfferDetailDto
                    {
                        MatchId = m.Id,
                        CandidateName = $"{m.Candidate.User.Name} ({m.Candidate.City})",
                        CandidateCity = m.Candidate.City,
                        MatchScore = m.MatchScore,
                        Status = m.Status,
                        IsSelectedByAlgorithm = m.IsSelectedByAlgorithm
                    }).ToList()
                })
                .ToList();

            Console.WriteLine($"✅ MatchController.GetEmployerOffers - Returning offers for {groupedMatches.Count} jobs");
            return groupedMatches;
        }

        // מחלקה לבקשת עדכון סטטוס
        public class UpdateStatusRequest
        {
            public string Status { get; set; }
        }

        public class ApplyRequest
        {
            public int JobId { get; set; }
        }
    }
}
