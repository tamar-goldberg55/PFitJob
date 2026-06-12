using Microsoft.AspNetCore.Mvc;
using Service.Dto;
using Service.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly IMatch _matchService;

        public MatchController(IMatch matchService)
        {
            _matchService = matchService;
        }

        // קבלת כל ההתאמות
        [HttpGet]
        public async Task<List<MatchDto>> Get()
        {
            return await _matchService.GetAll();
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

        // קבלת ההתאמה הכי טובה עבור מועמד ספציפי (לפי האלגוריתם)
        // GET: api/Match/candidate/5
        [HttpGet("candidate/{candidateId}")]
        public async Task<List<MatchDto>> GetTopMatches(int candidateId, [FromQuery] int topCount = 1)
        {
            return await _matchService.GetTopMatchesForCandidate(candidateId, topCount);
        }

        // קבלת מדד שביעות רצון כללי של המערכת
        // GET: api/Match/satisfaction
        [HttpGet("satisfaction")]
        public async Task<double> GetSatisfactionRate()
        {
            return await _matchService.GetGlobalSatisfactionRate();
        }

        // חישוב ציון התאמה תיאורטי בין מועמד למשרה
        // GET: api/Match/score?candidateId=1&jobId=2
        [HttpGet("score")]
        public async Task<double> GetMatchScore([FromQuery] int candidateId, [FromQuery] int jobId)
        {
            return await _matchService.CalculateMatchScore(candidateId, jobId);
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
}
