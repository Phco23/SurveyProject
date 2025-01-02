using Microsoft.EntityFrameworkCore;
using SurveyProject.Migrations;
using SurveyProject.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ContestRepository : IContestRepository
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

    public async Task<WinnerModel> AddWinnerAsync(WinnerModel winner)
    {
        _context.Winners.Add(winner);
        await _context.SaveChangesAsync();
        return winner;
    }

    public async Task<IEnumerable<WinnerModel>> GetWinnersByContestIdAsync(int contestId)
    {
        return await _context.Winners
            .Where(w => w.ContestId == contestId)
            .ToListAsync();
    }
}
