using Microsoft.AspNetCore.Mvc;
using SurveyProject.Models;
using SurveyProject.Repository;

[ApiController]
[Route("api/[controller]")]
public class ContestsController : ControllerBase
{
    private readonly IContestService _contestService;

    public ContestsController(IContestService contestService)
    {
        _contestService = contestService;
    }

    // 1. Add New Contest
    [HttpPost("add-contest")]
    public async Task<IActionResult> AddContest([FromBody] ContestModel contest)
    {
        if (contest == null || string.IsNullOrWhiteSpace(contest.Title))
        {
            return BadRequest("Invalid contest data.");
        }

        var newContest = await _contestService.AddContestAsync(contest);
        return CreatedAtAction(nameof(GetContestById), new { id = newContest.Id }, newContest);
    }

    // 2. Get Contest by ID (to complement CreatedAtAction)
    [HttpGet("{id}")]
    public async Task<IActionResult> GetContestById(int id)
    {
        var contest = await _contestService.GetContestByIdAsync(id);
        if (contest == null)
        {
            return NotFound("Contest not found.");
        }

        return Ok(contest);
    }

    // 3. Add New Winner
    [HttpPost("add-winner")]
    public async Task<IActionResult> AddWinner([FromBody] WinnerModel winner)
    {
        if (winner == null || string.IsNullOrWhiteSpace(winner.Name) || winner.ContestId <= 0)
        {
            return BadRequest("Invalid winner data.");
        }

        var newWinner = await _contestService.AddWinnerAsync(winner);
        return CreatedAtAction(nameof(GetWinnerById), new { id = newWinner.Id }, newWinner);
    }

    // 4. Get Winner by ID (to complement CreatedAtAction)
    [HttpGet("winner/{id}")]
    public async Task<IActionResult> GetWinnerById(int id)
    {
        var winner = await _contestService.GetWinnerByIdAsync(id);
        if (winner == null)
        {
            return NotFound("Winner not found.");
        }

        return Ok(winner);
    }

    // 5. Get Winners by Contest ID
    [HttpGet("contest/{contestId}/winners")]
    public async Task<IActionResult> GetWinnersByContestId(int contestId)
    {
        if (contestId <= 0)
        {
            return BadRequest("Invalid contest ID.");
        }

        var winners = await _contestService.GetWinnersByContestIdAsync(contestId);
        if (winners == null || !winners.Any())
        {
            return NotFound("No winners found for the specified contest.");
        }

        return Ok(winners);
    }
}
