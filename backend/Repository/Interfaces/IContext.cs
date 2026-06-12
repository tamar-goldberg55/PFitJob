
using Microsoft.EntityFrameworkCore;
using Repository.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IContext
    {

        DbSet<CandidateProfiles> CandidateProfiles { get; set; }
        DbSet<Categories> Categories { get; set; }
        DbSet<User> Users { get; set; }
         DbSet<Employer> Employers { get; set; }
         DbSet<JobListings> JobListings { get; set; }
         DbSet<Match> Match { get; set; }//אותיות אותו דבר 
       

        public void save();
    }
}
