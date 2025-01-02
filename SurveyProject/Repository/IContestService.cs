using SurveyProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IContestService
{
    Task<ContestModel> AddContestAsync(ContestModel contest);
    Task<WinnerModel> AddWinnerAsync(WinnerModel winner);
    Task<IEnumerable<WinnerModel>> GetWinnersByContestIdAsync(int contestId);
}
