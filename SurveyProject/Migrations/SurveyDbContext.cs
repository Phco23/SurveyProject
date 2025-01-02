using Microsoft.EntityFrameworkCore;
using SurveyProject.Models;
using System.Reflection;
using System;

namespace SurveyProject.Migrations
{
    using Microsoft.EntityFrameworkCore;

    public class SurveyDbContext : DbContext
    {
        public SurveyDbContext(DbContextOptions<SurveyDbContext> options) : base(options) { }

        public DbSet<ContestModel> Contests { get; set; }
        public DbSet<WinnerModel> Winners { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WinnerModel>()
                .HasOne(w => w.Contest)
                .WithMany(c => c.Winners)
                .HasForeignKey(w => w.ContestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
