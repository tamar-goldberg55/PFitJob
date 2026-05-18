using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.models;
using Service.Dto;
using Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _userService;

        public UserController(IUser userService)
        {
            _userService = userService;
        }

        // הרשמה כמתמודד (מועמד)
        [HttpPost("register/candidate")]
        public Task<string> RegisterCandidate([FromBody] UserDto userDto, [FromQuery]  string password)
        {
            return _userService.RegisterCandidateAsync(userDto, password);
        }

        // הרשמה כמעסיק
        [HttpPost("register/employer")]
        public Task<string> RegisterEmployer([FromBody] UserDto userDto, [FromQuery] string password)
        {
            return _userService.RegisterEmployerAsync(userDto, password);
        }

        // התחברות למערכת
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromQuery] string email, [FromQuery] string password)
        {
            try
            {
                // חשוב להשתמש ב-await כי הפונקציה ב-Service היא async
                var token = await _userService.LoginAsync(email, password);

                if (string.IsNullOrEmpty(token))
                {
                    // אם חזר null, נחזיר שגיאה 401 עם הסבר
                    return Unauthorized("אימייל או סיסמה לא נכונים");
                }

                return Ok(token); // אם הכל טוב, נחזיר 200 עם הטוקן
            }
            catch (Exception ex)
            {
                return BadRequest($"שגיאה בהתחברות: {ex.Message}");
            }
        }

        // קבלת כל המשתמשים
        [HttpGet]
        public async Task<List<UserDto>> GetAll()
        {
            return await _userService.GetAll();
        }

        // קבלת משתמש לפי ID
        [HttpGet("{id}")]
        public Task<UserDto> GetById(int id)
        {
            return _userService.GetById(id);
        }

        // מחיקת משתמש
        [HttpDelete("{id}")]
        public Task Delete(int id)
        {
            return _userService.DeleteItem(id);
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"]
                .ToString().Replace("Bearer ", "");

            await _userService.LogoutAsync(token);
            return Ok(new { message = "Logged out successfully" });
        }
    }
}
