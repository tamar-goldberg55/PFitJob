using Repository.Interfaces;
using Repository.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DataRepositories
{
    public class MatchRepository : IRepository<Match>
    {
        private readonly IContext _context;
        public MatchRepository(IContext context)
        {
            _context = context;
        }
       
        public async Task<Match> AddItem(Match item)
        {
            await _context.Match.AddAsync(item);
            _context.save();
            return item;
        }

        public async Task DeleteItem(int id)
        {
            _context.Match.Remove(await GetById(id));
            _context.save();
        }

        public Task<List<Match>> GetAll()
        {
            return _context.Match.ToListAsync();
        }

        public Task<Match> GetById(int id)
        {
            return _context.Match.ToList().FirstOrDefaultAsync(x => x.Id == id);
        }

        public  async Task UpdateItem(int id, Match item)
        {
            var match = GetById(id);
            match.Name = item.Name;
            _context.save();
        }
    }
}
