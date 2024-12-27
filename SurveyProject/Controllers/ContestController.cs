using Microsoft.AspNetCore.Mvc;
using SurveyProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ContestsController : ControllerBase
{
    private readonly IContestService _contestService;

    public ContestsController(IContestService contestService)
    {
        _contestService = contestService;
    }

    [HttpPost("add-contest")]
    public async Task<IActionResult> AddContest([FromBody] ContestModel contest)
    {
        if (contest == null || string.IsNullOrWhiteSpace(contest.Title))
        {
            return BadRequest("Invalid contest data.");
        }

        var createdContest = await _contestService.AddContestAsync(contest);
        return CreatedAtAction(nameof(GetContestById), new { id = createdContest.Id }, createdContest);
    }

    [HttpPost("add-winner")]
    public async Task<IActionResult> AddWinner([FromBody] WinnerModel winner)
    {
        if (winner == null || string.IsNullOrWhiteSpace(winner.Name))
        {
            return BadRequest("Invalid winner data.");
        }

        var createdWinner = await _contestService.AddWinnerAsync(winner);
        return Ok(createdWinner);
    }

    [HttpGet("winners/{contestId}")]
    public async Task<IActionResult> GetWinners(int contestId)
    {
        var winners = await _contestService.GetWinnersByContestIdAsync(contestId);
        if (winners == null)
        {
            return NotFound();
        }

        return Ok(winners);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContestById(int id)
    {
        // Add logic to retrieve a contest by ID if needed
        return Ok();
    }
}
