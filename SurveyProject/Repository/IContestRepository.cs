using SurveyProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyProject.Repository
{
    public interface IContestRepository
    {
        Task<ContestModel> AddContestAsync(ContestModel contest);
        Task<ContestModel?> GetContestByIdAsync(int id);
        Task<WinnerModel> AddWinnerAsync(WinnerModel winner);
        Task<WinnerModel?> GetWinnerByIdAsync(int id);
        Task<IEnumerable<WinnerModel>> GetWinnersByContestIdAsync(int contestId);
    }
}
