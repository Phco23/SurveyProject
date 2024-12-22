using Microsoft.AspNetCore.Mvc;
using SurveyProject.Models;
using SurveyProject.Repository;
using System;
using System.Reflection;

namespace SurveyProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContestsController : ControllerBase
    {
        private readonly IWinnerService _contestService;

        public ContestsController(IWinnerService contestService)
        {
            _contestService = contestService;
        }

        // Add a winner to a contest
        [HttpPost("{contestId}/winners")]
        public async Task<IActionResult> AddWinner(int contestId, [FromBody] WinnerModel winner)
        {
            if (winner == null)
            {
                return BadRequest("Winner data is required.");
            }

            winner.ContestId = contestId;
            var createdWinner = await _contestService.AddWinnerAsync(winner);
            return CreatedAtAction(nameof(GetWinnersByContestId), new { contestId = contestId }, createdWinner);
        }

        // Get winners of a contest
        [HttpGet("{contestId}/winners")]
        public async Task<IActionResult> GetWinnersByContestId(int contestId)
        {
            var winners = await _contestService.GetWinnersByContestIdAsync(contestId);
            return Ok(winners);
        }
    }
}
