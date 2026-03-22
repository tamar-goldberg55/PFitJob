using Microsoft.AspNetCore.Mvc;
using Service.Dto;
using Service.Interfaces;
using Repository.models;
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
        public Task<string> RegisterCandidate([FromBody] UserDto userDto, string password)
        {
            return _userService.RegisterCandidateAsync(userDto, password);
        }

        // הרשמה כמעסיק
        [HttpPost("register/employer")]
        public Task<string> RegisterEmployer([FromBody] UserDto userDto, string password)
        {
            return _userService.RegisterEmployerAsync(userDto, password);
        }

        // התחברות למערכת
        //[HttpPost("login")]
        //public Task<string> Login(string email, string password)
        //{
        //    return _userService.LoginAsync(email, password);
        //}
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(string email, string password)
        {
            // חשוב להשתמש ב-await כי הפונקציה ב-Service היא async
            var token = await _userService.LoginAsync(email, password);

            if (token == null)
            {
                // אם חזר null, נחזיר שגיאה 401 עם הסבר
                return Unauthorized("אימייל או סיסמה לא נכונים");
            }

            return Ok(token); // אם הכל טוב, נחזיר 200 עם הטוקן
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
    }
}