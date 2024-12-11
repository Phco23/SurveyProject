using SurveyProject.Models;
using System;

namespace SurveyProject.Repository
{
    public class ContestService : IContestService
    {
        private readonly IContestRepository _contestRepository;

        public ContestService(IContestRepository contestRepository)
        {
            _contestRepository = contestRepository;
        }

        public async Task<ContestModel> AddContestAsync(ContestModel contest)
        {
            if (string.IsNullOrWhiteSpace(contest.Title))
            {
                throw new ArgumentException("Contest title cannot be empty.");
            }

            return await _contestRepository.AddContestAsync(contest);
        }

        public async Task<ContestModel?> GetContestByIdAsync(int id)
        {
            return await _contestRepository.GetContestByIdAsync(id);
        }
    }

}
