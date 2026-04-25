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
    public class EmployerRepository : IRepositoryEmployer

    {
        private readonly IContext _context;

        public EmployerRepository(IContext context)
        {
            _context = context;

        }
        public async Task<Employer> AddItem(Employer item)
        {
            await _context.Employers.AddAsync(item);
            _context.save();
            return item;
        }

        public  async Task DeleteItem(int id)
        {
            _context.Employers.Remove(await GetById(id));
            _context.save();
        }

        public Task<List<Employer>> GetAll()
        {
            return _context.Employers.ToListAsync();
        }

        public async Task<Employer?> GetById(int id)
        {
            //return _context.Employers.ToList().FirstOrDefaultAsync(x => x.Id == id);
            return await _context.Employers
                 .Include(e => e.MyJobs) // שורה קריטית!
                    .FirstOrDefaultAsync(e => e.Id == id);
        }
        public async Task<Employer> GetByUserId(int UserId)
        {
            return await _context.Employers
                            .Include(e => e.MyJobs) // שורה קריטית!
                               .FirstOrDefaultAsync(e => e.UserId == UserId);
        }

        public async Task UpdateItem(int id, Employer item)
        {
            var employee = await GetById(id);
            employee.CompanyName = item.CompanyName;
            employee.status = item.status;
            _context.save();
        }

      
    }
}
