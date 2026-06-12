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
    public class CandidateProfilesRepository : IRepository<CandidateProfiles>
    {
        private readonly IContext _context;
        public CandidateProfilesRepository(IContext context)
        {
            _context = context;
        }
        public async Task<CandidateProfiles> AddItem(CandidateProfiles item)
        {
            await _context.CandidateProfiles.AddAsync(item);
            _context.save();
            return item;
        }

        public  async Task DeleteItem(int id)
        {
            _context.CandidateProfiles.Remove(await GetById(id));
            _context.save();
        }

        public Task<List<CandidateProfiles>> GetAll()
        {
            return _context.CandidateProfiles.ToListAsync();
        }

        public Task<CandidateProfiles> GetById(int id)
        {
            return  _context.CandidateProfiles.FirstOrDefaultAsync(x => x.Id == id);
        }

        public  async Task UpdateItem(int id, CandidateProfiles item)
        {
            //var CandidateProfile = await GetById(id);
            //CandidateProfile.Name = item.Name;//לבדוק את זה 
            //_context.save();
            var existingProfile = await GetById(id);

            if (existingProfile != null)
            {
                // עדכון השדות בהתאם למודל CandidateProfiles
                existingProfile.City = item.City;
                existingProfile.MaxDistance = item.MaxDistance;
                existingProfile.MinHourlyRate = item.MinHourlyRate;
                existingProfile.activity = item.activity;
                existingProfile.level = item.level;
                existingProfile.IsRemoteOnly = item.IsRemoteOnly;
                existingProfile.Withpepole = item.Withpepole;

                // הערה: בדרך כלל לא מעדכנים את ה-UserId או ה-Id עצמו

                _context.save();
            }
        }
    }
}
