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
    public class JobListingsExtendedRepository : JobListingsRepository
    {
        private readonly IContext _context;

        public JobListingsExtendedRepository(IContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ToggleJobStatus(int jobId, bool isActive)
        {
            var job = await _context.JobListings.FirstOrDefaultAsync(j => j.Id == jobId);
            if (job == null) return false;

            job.IsCatch = isActive;
            _context.save();
            return true;
        }

        public async Task<List<JobListings>> GetJobByEmployer(int empId)
        {
            var jobs = await _context.JobListings
                .Where(job => job.EmployerId == empId)
                .ToListAsync();

            return jobs;
        }

        public async Task<List<JobListings>> GetJobByEmployerWithMatches(int empId)
        {
            Console.WriteLine($"🔍 JobListingsExtendedRepository.GetJobByEmployerWithMatches - EmployerId: {empId}");

            // שימוש ב-_context ישירות עם Include למאצ'ים
            var jobsWithMatches = await _context.JobListings
                .Include(j => j.Matches) // צירוף המאצ'ים המקושרים
                .Include(j => j.Employer) // צירוף פרטי המעסיק
                .Where(job => job.EmployerId == empId)
                .ToListAsync();

            Console.WriteLine($"📊 JobListingsExtendedRepository - Found {jobsWithMatches.Count} jobs for employer {empId}");
            Console.WriteLine($"✅ JobListingsExtendedRepository - Returning {jobsWithMatches.Count} jobs with matches");
            return jobsWithMatches;
        }
        public async Task<List<JobListings>> GetJobsWithApplicants(int empId)
        {
            return await _context.JobListings
                .Include(j => j.Matches)
                    .ThenInclude(m => m.Candidate)
                        .ThenInclude(c => c.User) // כדי לקבל את שם המועמד מהטבלה של המשתמשים
                .Where(job => job.EmployerId == empId)
                .ToListAsync();
        }
    }
}
