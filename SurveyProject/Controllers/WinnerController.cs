using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class WinnersController : ControllerBase
{
    private readonly IContestService _contestService;

    public WinnersController(IContestService contestService)
    {
        _contestService = contestService;
    }

    [HttpGet("{contestId}")]
    public async Task<IActionResult> GetWinnersByContestId(int contestId)
    {
        if (contestId <= 0)
        {
            return BadRequest("Invalid contest ID.");
        }

        var winners = await _contestService.GetWinnersByContestIdAsync(contestId);
        if (winners == null || !winners.Any())
        {
            return NotFound($"No winners found for contest ID {contestId}.");
        }

        return Ok(winners);
    }
}
