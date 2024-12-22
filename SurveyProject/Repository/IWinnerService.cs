using SurveyProject.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyProject.Repository
{
    public interface IWinnerService
    {
        Task<WinnerModel> AddWinnerAsync(WinnerModel winner);
        Task<IEnumerable<WinnerModel>> GetWinnersByContestIdAsync(int contestId);
    }

    public class WinnerService : IWinnerService
    {
        private readonly IWinnerRepository _contestRepository;

        public WinnerService(IWinnerRepository contestRepository)
        {
            _contestRepository = contestRepository ?? throw new ArgumentNullException(nameof(contestRepository));
        }

        public async Task<WinnerModel> AddWinnerAsync(WinnerModel winner)
        {
            if (winner == null)
                throw new ArgumentNullException(nameof(winner), "Winner cannot be null.");

            if (string.IsNullOrWhiteSpace(winner.Name))
                throw new ArgumentException("Winner name cannot be empty.", nameof(winner.Name));

            if (string.IsNullOrWhiteSpace(winner.RollNumber))
                throw new ArgumentException("Winner roll number cannot be empty.", nameof(winner.RollNumber));

            return await _contestRepository.AddWinnerAsync(winner);
        }

        public async Task<IEnumerable<WinnerModel>> GetWinnersByContestIdAsync(int contestId)
        {
            if (contestId <= 0)
                throw new ArgumentException("Contest ID must be greater than zero.", nameof(contestId));

            return await _contestRepository.GetWinnersByContestIdAsync(contestId);
        }
    }
}
