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
            // הגדרה שאומרת: כשמוחקים משרה, אל תמחק אוטומטית את ההתאמות (Match)
            modelBuilder.Entity<Match>()
                .HasOne(m => m.Job) // ודאי שיש לך מאפיין כזה בתוך מחלקת Match
                .WithMany()
                .HasForeignKey(m => m.JobId)
                .OnDelete(DeleteBehavior.Restrict); // זה הפתרון!

            // אפשר להוסיף אותו דבר גם למועמד אם השגיאה נמשכת
            modelBuilder.Entity<Match>()
                .HasOne(m => m.Candidate)
                .WithMany()
                .HasForeignKey(m => m.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Match>()
        //        .HasOne(m => m.Job)
        //        .WithMany(j => j.Matches)
        //        .HasForeignKey(m => m.JobId)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    modelBuilder.Entity<Match>()
        //        .HasOne(m => m.Candidate)
        //        .WithMany(c => c.Matches)
        //        .HasForeignKey(m => m.CandidateId)
        //        .OnDelete(DeleteBehavior.Restrict);
        //}

        //צריך פה קישור לSQL וכזה 
        //צריך להריץ
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    // פתרון השגיאה: ביטול מחיקה אוטומטית בטבלת Match
        //    modelBuilder.Entity<Match>()
        //        .HasOne(m => m.Job)
        //        .WithMany()
        //        .HasForeignKey(m => m.JobId)
        //        .OnDelete(DeleteBehavior.NoAction); // חשוב מאוד!

        //    modelBuilder.Entity<Match>()
        //        .HasOne(m => m.Candidate)
        //        .WithMany()
        //        .HasForeignKey(m => m.CandidateId)
        //        .OnDelete(DeleteBehavior.NoAction); // חשוב מאוד!

        //    // פתרון האזהרה של ה-Decimal
        //    modelBuilder.Entity<CandidateProfiles>()
        //        .Property(p => p.MinHourlyRate)
        //        .HasColumnType("decimal(18,2)");

        //    modelBuilder.Entity<JobListings>()
        //        .Property(p => p.Payment)
        //        .HasColumnType("decimal(18,2)");
        //}
    }
}
