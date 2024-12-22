using Microsoft.EntityFrameworkCore;
using SurveyProject.Models;
using System.Reflection;
using System;

namespace SurveyProject.Migrations
{
    public class SurveyDbContext : DbContext
    {
        public SurveyDbContext(DbContextOptions<SurveyDbContext> options) : base(options) { }

        public DbSet<ContestModel> Contests { get; set; }
        public DbSet<WinnerModel> Winners { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WinnerModel>(entity =>
            {
                entity.HasKey(w => w.Id);
                entity.Property(w => w.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(w => w.RollNumber)
                    .IsRequired()
                    .HasMaxLength(20);
                entity.HasOne(w => w.Contest)
                    .WithMany(c => c.Winners)
                    .HasForeignKey(w => w.ContestId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ContestModel>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Title)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(c => c.Description)
                    .HasMaxLength(1000);
            });
        }
    }
}
