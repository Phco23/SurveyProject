using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SurveyProject.Models;
using System.Drawing;

namespace SurveyProject.Repository
{
    public class DataContext : IdentityDbContext<IdentityUserModel>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }



        public DbSet<SurveyModel> Surveys { get; set; }
        public DbSet<ResponseModel> Responses { get; set; }
        public DbSet<QuestionModel> Questions { get; set; }
        public DbSet<OptionModel> Options { get; set; }
        public DbSet<QuestionTypeModel> QuestionTypes { get; set; }
        public DbSet<ResponseDetailsModel> ResponseDetails { get; set; }
        public DbSet<FAQ> FAQs {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<QuestionModel>()
            .HasOne(q => q.Survey)
            .WithMany(s => s.Questions)
            .HasForeignKey(q => q.SurveyId);

            modelBuilder.Entity<OptionModel>()
                .HasOne(o => o.Question)
                .WithMany(q => q.Options)
                .HasForeignKey(o => o.QuestionId);

            modelBuilder.Entity<ResponseDetailsModel>()
                .HasOne(rd => rd.Response)
                .WithMany()
                .HasForeignKey(rd => rd.ResponseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ResponseDetailsModel>()
                .HasOne(rd => rd.Question)
                .WithMany()
                .HasForeignKey(rd => rd.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ResponseDetailsModel>()
                .HasOne(rd => rd.Option)
                .WithMany()
                .HasForeignKey(rd => rd.OptionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
