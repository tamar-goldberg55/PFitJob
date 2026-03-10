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
        public  async Task<JobListings> AddItem(JobListings item)
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

        public Task<List<JobListings>> GetAll()
        {
            return _context.JobListings.ToListAsync();
        }

        public Task<JobListings> GetById(int id)
        {
            return _context.JobListings.ToList().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateItem(int id, JobListings item)
        {
            var jobListing = GetById(id);
            category.Name = item.Name;
            _context.save();
        }
    }
}
