using Repository.Interfaces;
using Repository.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DataRepositories
{
    
    public class UserRepository : IRepository<User>
    {
        private readonly IContext _context;
        public UserRepository(IContext context)
        {
            _context = context;
        }
       
        public async Task<User> AddItem(User item)
        {
            await _context.Users.AddAsync(item);
            _context.save();
            return item;
        }

        public async Task DeleteItem(int id)
        {
            _context.Users.Remove(await GetById(id));
            _context.save();
        }

        public Task<List<User>> GetAll()
        {
            return _context.Users.ToListAsync();
        }

        public Task<User> GetById(int id)
        {
            return _context.Users.ToList().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateItem(int id, User item)
        {
            var user = GetById(id);
            user.Name = item.Name;
            _context.save();
        }
    }
}
