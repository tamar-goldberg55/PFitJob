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
    public class JobListingsRepository : IRepository<JobListings>
    {
        private readonly IContext _context;
        public JobListingsRepository(IContext context)
        {
            _context = context;
        }
        public async Task<JobListings> AddItem(JobListings item)
        {
            await _context.JobListings.AddAsync(item);
            _context.save();
            return item;
        }

        public async Task DeleteItem(int id)
        {
            _context.JobListings.Remove(await GetById(id));
            _context.save();
        }

        public  Task<List<JobListings>> GetAll()
        {
            return  _context.JobListings.ToListAsync();
        }

        public Task<JobListings> GetById(int id)
        {
            //return _context.JobListings.ToList().FirstOrDefaultAsync(x => x.Id == id);
            return  _context.JobListings.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateItem(int id, JobListings item)
        {
            var jobListing = await GetById(id);

            if (jobListing != null)
            {
                // 2. עדכון השדות - חשוב לעדכן את כל השדות שאת רוצה לשנות
                // במודל שלך לא היה Name, אז הנה דוגמא לשדה שקיים (כמו Title)
                jobListing.Title = item.Title;
                jobListing.Description = item.Description;
                jobListing.IsCatch = item.IsCatch; // זה הסטטוס שרצית
                jobListing.Payment = item.Payment;

                // 3. שמירה מסונכרנת (אופציונלי להפוך ל-Async גם ב-Context)
                _context.save();
            }
        }
    }
}