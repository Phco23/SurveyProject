using Microsoft.EntityFrameworkCore;
using SurveyProject.Migrations;
using SurveyProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SurveyProject.Repository
{
    public interface IWinnerRepository
    {
        Task<WinnerModel> AddWinnerAsync(WinnerModel winnerModel);
        Task<IEnumerable<WinnerModel>> GetWinnersByContestIdAsync(int contestId);
    }

    public class WinnerRepository : IWinnerRepository
    {
        private readonly SurveyDbContext _context;

        public WinnerRepository(SurveyDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<WinnerModel> AddWinnerAsync(WinnerModel winnerModel)
        {
            if (winnerModel == null)
                throw new ArgumentNullException(nameof(winnerModel), "Winner model cannot be null.");

            var winnerEntity = new WinnerModel
            {
                Name = winnerModel.Name,
                RollNumber = winnerModel.RollNumber,
                ContestId = winnerModel.ContestId
            };

            _context.Winners.Add(winnerEntity);
            await _context.SaveChangesAsync();

            return winnerEntity;
        }

        public async Task<IEnumerable<WinnerModel>> GetWinnersByContestIdAsync(int contestId)
        {
            if (contestId <= 0)
                throw new ArgumentException("Contest ID must be greater than zero.", nameof(contestId));

            return await _context.Winners
                .Where(w => w.ContestId == contestId)
                .Select(w => new WinnerModel
                {
                    Id = w.Id,
                    Name = w.Name,
                    RollNumber = w.RollNumber,
                    ContestId = w.ContestId
                })
                .ToListAsync();
        }
    }
}
