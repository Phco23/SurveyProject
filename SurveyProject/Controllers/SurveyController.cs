using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyProject.Models;
using SurveyProject.Models.ViewModels;
using SurveyProject.Repository;
using System.Security.Claims;

namespace SurveyProject.Controllers
{
    public class SurveyController : Controller
    {
        private readonly DataContext _context;

        public SurveyController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SurveySubmitted()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SubmitSurvey(IFormCollection form)
        {
            // Ensure the user is authenticated
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Extract SurveyId from the form data
            if (!int.TryParse(form["SurveyId"], out var surveyId))
                return BadRequest("Invalid SurveyId");

            // Create a new ResponseModel
            var response = new ResponseModel
            {
                SurveyId = surveyId,
                UserId = userId,
                SubmittedDate = DateTime.UtcNow
            };

            // Save the ResponseModel to get its ID
            _context.Responses.Add(response);
            await _context.SaveChangesAsync();

            // Initialize a list to store ResponseDetails
            var responseDetails = new List<ResponseDetailsModel>();

            // Process form data for each question
            foreach (var key in form.Keys.Where(k => k.StartsWith("Question_")))
            {
                var questionId = int.Parse(key.Split('_')[1]);
                var answers = form[key].ToString(); // Can be single or multiple values

                // Find the question and its type
                var question = await _context.Questions
                    .Include(q => q.QuestionType)
                    .FirstOrDefaultAsync(q => q.Id == questionId);

                if (question == null)
                {
                    // Log or handle invalid question
                    continue;
                }

                if (question.QuestionType.Name == "Radio Button" || question.QuestionType.Name == "Checkbox")
                {
                    // Handle choice-based questions
                    var optionIds = answers.Split(',')
                        .Where(a => int.TryParse(a, out _))
                        .Select(int.Parse)
                        .ToList();

                    // Validate option IDs
                    var validOptionIds = await _context.Options
                        .Where(o => optionIds.Contains(o.Id) && o.QuestionId == questionId)
                        .Select(o => o.Id)
                        .ToListAsync();

                    if (!validOptionIds.Any())
                    {
                        // Log or handle invalid options
                        continue;
                    }

                    responseDetails.AddRange(validOptionIds.Select(optionId => new ResponseDetailsModel
                    {
                        ResponseId = response.Id,
                        QuestionId = questionId,
                        OptionId = optionId
                    }));
                }
                else if (question.QuestionType.Name == "Text")
                {
                    // Handle text-based questions
                    responseDetails.Add(new ResponseDetailsModel
                    {
                        ResponseId = response.Id,
                        QuestionId = questionId,
                        AnswerText = answers,
                        OptionId = null // Ensure OptionId is null for text responses
                    });
                }
            }

            // Save ResponseDetails
            if (responseDetails.Any())
            {
                _context.ResponseDetails.AddRange(responseDetails);
                await _context.SaveChangesAsync();
            }

            // Redirect to a confirmation page
            return RedirectToAction("SurveySubmitted");
        }



        public async Task<IActionResult> SurveyDetails(int id)
        {
            var survey = await _context.Surveys
                .Include(s => s.Questions)
                .ThenInclude(q => q.Options)
                .Include(s => s.Questions)
                .ThenInclude(q => q.QuestionType)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (survey == null)
                return NotFound();

            var viewModel = new SurveyViewModel
            {
                SurveyId = survey.Id,
                Title = survey.Title,
                Description = survey.Description,
                ExpiredDate = survey.ExpiredDate,
                IsActive = survey.IsActive,
                Questions = survey.Questions.Select(q => new QuestionViewModel
                {
                    QuestionId = q.Id,
                    QuestionText = q.QuestionText,
                    QuestionType = q.QuestionType.Name, // Single Choice, Multiple Choice, Text
                    Options = q.Options?.Select(o => new OptionViewModel
                    {
                        OptionId = o.Id,
                        OptionText = o.OptionText
                    }).ToList()
                }).ToList()
            };

            return View(viewModel);
        }


        public async Task<IActionResult> ListSurvey()
        {
            return View(await _context.Surveys.ToListAsync());
        }
    }
}
