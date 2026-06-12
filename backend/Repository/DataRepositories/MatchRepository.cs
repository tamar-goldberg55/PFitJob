using Microsoft.EntityFrameworkCore;
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
            return _context.Match.FirstOrDefaultAsync(x => x.Id == id);
        }

        public  async Task UpdateItem(int id, Match item)
        {
            //var match =await GetById(id);
            //match.Name = item.Name;
            //_context.save();
            //--------
            var existingMatch = await GetById(id);

            // 2. בדיקה שהאובייקט אכן נמצא
            if (existingMatch != null)
            {
                // 3. עדכון השדות הרלוונטיים מהמודל Match
                existingMatch.MatchScore = item.MatchScore;
                existingMatch.MatchDate = item.MatchDate;

                // בדרך כלל לא נרצה לעדכן את ה-JobId או ה-CandidateId בתוך Update,
                // כי אם הם משתנים, זה כבר נחשב "Match" חדש לגמרי.

                // 4. שמירת השינויים
                _context.save();
            }
        }
    }
}
