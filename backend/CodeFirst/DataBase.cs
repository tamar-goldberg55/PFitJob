using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Repository.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFirst
{
    
    public class DataBase : DbContext, IContext
    {
        //חייב להיות?
        public DataBase(DbContextOptions<DataBase> options) : base(options)
        {
        }

        public DbSet<CandidateProfiles> CandidateProfiles { get; set; }
        public DbSet<Categories> Categories { get ; set; }
        public DbSet<User> Users { get ; set ; }
        public DbSet<Employer> Employers { get ; set ; }
        public DbSet<JobListings> JobListings { get; set ; }
        public DbSet<Match> Match { get ; set ; }
     


        public void save()
        {
            this.SaveChanges();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // כאן את כותבת את שורת החיבור שלך
                optionsBuilder.UseSqlServer("Server=DESKTOP-SVITPH4;Database=JobDataBase;Trusted_Connection=True;TrustServerCertificate=True;");
            }



        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Match>()
                .HasOne(m => m.Job)
                .WithMany(j => j.Matches)
                .HasForeignKey(m => m.JobId);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Candidate)
                .WithMany()
                .HasForeignKey(m => m.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CandidateProfiles>()
                .Property(p => p.MinHourlyRate)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<JobListings>()
                .Property(p => p.Payment)
                .HasColumnType("decimal(18,2)");
        }
    }
}
