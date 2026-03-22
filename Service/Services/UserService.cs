using AutoMapper;
using Repository.models;
using Service.Dto;
using Service.Interfaces;
using Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services
{
    // הערה: הסרתי את ההורשה מ-TokenService כי אנחנו מזריקים אותו בבנאי (Dependency Injection)
    public class UserService : IUser
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<CandidateProfiles> _candidateRepository;
        private readonly IRepository<Employer> _employerRepository;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public UserService(
            IRepository<User> userRepository,
            IRepository<CandidateProfiles> candidateRepository,
            IRepository<Employer> employerRepository,
            IMapper map,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _candidateRepository = candidateRepository;
            _employerRepository = employerRepository;
            _mapper = map;
            _tokenService = tokenService;
        }

        //public async Task<string> RegisterAsync(UserDto userDto, string password, UserRole role)
        //{
        //    // 1. הצפנת סיסמה
        //    string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        //    // 2. מיפוי ל-Entity
        //    User newUser = _mapper.Map<User>(userDto);
        //    newUser.PasswordHash = passwordHash;
        //    newUser.UserType = role; // ודאי שבמודל User קוראים לשדה UserType או Role
        //    newUser.IsEnable = true;

        //    // 3. שמירה בבסיס הנתונים
        //    var addedUser = await _userRepository.AddItem(newUser);

        //    // 4. יצירת פרופיל ריק לפי התפקיד
        //    if (role == UserRole.Candidate)
        //    {
        //        await _candidateRepository.AddItem(new CandidateProfiles { UserId = addedUser.Id });
        //    }
        //    else if (role == UserRole.Employer)
        //    {
        //        await _employerRepository.AddItem(new Employer { UserId = addedUser.Id });
        //    }

        //    return _tokenService.GenerateToken(addedUser);
        //}
        // הרשמה למועמד - השרת קובע שהתפקיד הוא Candidate
        public async Task<string> RegisterCandidateAsync(UserDto userDto, string password)
        {
            return await InternalRegister(userDto, password, UserRole.Candidate);
        }

        // הרשמה למעסיק - השרת קובע שהתפקיד הוא Employer
        public async Task<string> RegisterEmployerAsync(UserDto userDto, string password)
        {
            return await InternalRegister(userDto, password, UserRole.Employer);
        }

        // פונקציית עזר פרטית (private) שאליה המשתמש לא יכול לגשת ישירות
        private async Task<string> InternalRegister(UserDto userDto, string password, UserRole role)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            User newUser = _mapper.Map<User>(userDto);
            newUser.PasswordHash = passwordHash;
            newUser.UserType = role;
            newUser.IsEnable = true;

            var addedUser = await _userRepository.AddItem(newUser);

            if (role == UserRole.Candidate)
            {
                await _candidateRepository.AddItem(new CandidateProfiles { UserId = addedUser.Id });
            }
            else if (role == UserRole.Employer)
            {
                await _employerRepository.AddItem(new Employer { UserId = addedUser.Id });
            }

            return _tokenService.GenerateToken(addedUser);
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var allUsers = await _userRepository.GetAll();
            var user = allUsers.FirstOrDefault(u => u.Email == email);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return _tokenService.GenerateToken(user);
            }
            return null;
        }

        public async Task<List<UserDto>> GetAll()
        {
            var users = await _userRepository.GetAll();
            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<UserDto> GetById(int id)
        {
            var user = await _userRepository.GetById(id);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> AddItem(UserDto item)
        {
            var entity = _mapper.Map<User>(item);
            var added = await _userRepository.AddItem(entity);
            return _mapper.Map<UserDto>(added);
        }

        public async Task UpdateItem(int id, UserDto item)
        {
            var userEntity = _mapper.Map<User>(item);
            await _userRepository.UpdateItem(id, userEntity);
        }

        public async Task DeleteItem(int id)
        {
            await _userRepository.DeleteItem(id);
        }
    }
}
