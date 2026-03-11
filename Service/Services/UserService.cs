using AutoMapper;
using Repository.models;
using Service.Dto;
using Service.Interfaces;
using Repository.DataRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Interfaces;

namespace Service.Services
{
    public class UserService : TokenService, IUser
    {
        private readonly IRepository<User> _repository;
        private readonly IMapper mapper;
        private readonly ITokenService _tokenService; // הוספנו את השירות של הטוקן
        public UserService(IRepository<User> repository, IMapper map, ITokenService tokenService)
        {
            this._repository = repository;
            this.mapper = map;
            this._tokenService = tokenService;
        }
        // הרישום מחזיר עכשיו Token במקום UserDto
        public async Task<string> RegisterAsync(UserDto userDto, string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            User newUser = mapper.Map<UserDto, User>(userDto);
            newUser.PasswordHash = passwordHash;

            var addedUser = await _repository.AddItem(newUser);

            // אחרי שהמשתמש נוצר, יוצרים לו טוקן
            return _tokenService.GenerateToken(addedUser);
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            // 1. שליפת כל המשתמשים (או המרה ל-List)
            var allUsers = await _repository.GetAll();

            // 2. מציאת המשתמש הספציפי עם LINQ
            var user = allUsers.FirstOrDefault(u => u.Email == email);

            // 3. בדיקת הסיסמה כפי שעשינו קודם
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return _tokenService.GenerateToken(user);
            }
            return null;
        }

        public Task<List<UserDto>> GetAll()//מתי מוסיפים async 
        {
            return mapper.Map<Task<List<User>>, Task<List<UserDto>>>(_repository.GetAll());
        }

        public async Task<UserDto> GetById(int id)
        {
            return mapper.Map<User, UserDto>(await _repository.GetById(id));
        }

        public async Task<UserDto> AddItem(UserDto item)
        {
            return mapper.Map<User, UserDto>(
         await _repository.AddItem(mapper.Map<UserDto, User>(item)));

        }

        public Task UpdateItem(int id, UserDto item)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteItem(int id)
        {
            await _repository.DeleteItem(id);
        }


    }
};
