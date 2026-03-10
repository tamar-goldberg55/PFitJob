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
    public class UserService : IUser
    {
        private readonly IRepository<User> _repository;
        private readonly IMapper mapper;
        public UserService(IRepository<User> repository, IMapper map)
        {
            this._repository = repository;
            this.mapper = map;
        }
        public Task<UserDto> RegisterAsync(UserDto userDto, string password)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> LoginAsync(string email, string password)
        {
            throw new NotImplementedException();
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
