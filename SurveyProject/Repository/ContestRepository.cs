using SurveyProject.Migrations;
using SurveyProject.Models;
using System;

namespace SurveyProject.Repository
{
    public class ContestRepository : IWinnerRepository
    {
        private readonly SurveyDbContext _context;

        public ContestRepository(SurveyDbContext context)
        {
            _context = context;
        }

        public async Task<ContestModel> AddContestAsync(ContestModel contest)
        {
            _context.Contests.Add(contest);
            await _context.SaveChangesAsync();
            return contest;
        }

        public async Task<ContestModel?> GetContestByIdAsync(int id)
        {
            return await _context.Contests.FindAsync(id);
        }
    }
}
