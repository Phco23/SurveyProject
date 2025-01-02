using SurveyProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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

    public async Task<WinnerModel> AddWinnerAsync(WinnerModel winner)
    {
        if (string.IsNullOrWhiteSpace(winner.Name) || winner.ContestId <= 0)
        {
            throw new ArgumentException("Invalid winner data.");
        }

        return await _contestRepository.AddWinnerAsync(winner);
    }

    public async Task<IEnumerable<WinnerModel>> GetWinnersByContestIdAsync(int contestId)
    {
        return await _contestRepository.GetWinnersByContestIdAsync(contestId);
    }
}
