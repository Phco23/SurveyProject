using Microsoft.AspNetCore.Mvc;
using SurveyProject.Models;
using SurveyProject.Repository;
using System;

namespace SurveyProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContestsController : ControllerBase
    {
        private readonly IContestService _contestService;

        public ContestsController(IContestService contestService)
        {
            _contestService = contestService;
        }

        [HttpPost]
        public async Task<IActionResult> AddContest([FromBody] ContestModel contest)
        {
            if (contest == null)
            {
                return BadRequest("Contest data is required.");
            }

            var createdContest = await _contestService.AddContestAsync(contest);
            return CreatedAtAction(nameof(GetContestById), new { id = createdContest.Id }, createdContest);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContestById(int id)
        {
            var contest = await _contestService.GetContestByIdAsync(id);
            if (contest == null)
            {
                return NotFound();
            }

            return Ok(contest);
        }
    }
}
