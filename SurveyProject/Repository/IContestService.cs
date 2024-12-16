using SurveyProject.Models;
using System;

namespace SurveyProject.Repository
{
    public interface IContestService
    {
        Task<ContestModel> AddContestAsync(ContestModel contest);
        Task<ContestModel?> GetContestByIdAsync(int id);
    }

}
