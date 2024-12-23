using Microsoft.EntityFrameworkCore;
using SurveyProject.Models;

namespace SurveyProject.Migrations
{
    public class SurveyDbContext : DbContext
    {
        public SurveyDbContext(DbContextOptions<SurveyDbContext> options) : base(options) { }

        public DbSet<ContestModel> contests { get; set; }
    }
}
