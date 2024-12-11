using SurveyProject.Models;
using System;

namespace SurveyProject.Repository
{
    public interface IContestRepository
    {
        Task<ContestModel> AddContestAsync(ContestModel contest);
        Task<ContestModel?> GetContestByIdAsync(int id);
    }
}
