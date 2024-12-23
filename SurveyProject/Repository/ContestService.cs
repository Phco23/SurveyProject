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
        return await _contestRepository.AddContestAsync(contest);
    }

    public async Task<ContestModel?> GetContestByIdAsync(int id)
    {
        return await _contestRepository.GetContestByIdAsync(id);
    }

    public async Task<WinnerModel> AddWinnerAsync(WinnerModel winner)
    {
        return await _contestRepository.AddWinnerAsync(winner);
    }

    public async Task<WinnerModel?> GetWinnerByIdAsync(int id)
    {
        return await _contestRepository.GetWinnerByIdAsync(id);
    }

    public async Task<IEnumerable<WinnerModel>> GetWinnersByContestIdAsync(int contestId)
    {
        return await _contestRepository.GetWinnersByContestIdAsync(contestId);
    }
}
}
