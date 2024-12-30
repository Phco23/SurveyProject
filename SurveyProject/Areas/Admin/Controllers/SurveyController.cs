using eLearning.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SurveyProject.MappingProfiles;
using SurveyProject.Models;
using SurveyProject.Models.ViewModels;
using SurveyProject.Repository;
using System.Data;

namespace SurveyProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SurveyController : Controller
    {
        private readonly UserManager<IdentityUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;
        private readonly EmailService _emailService;

        public SurveyController(UserManager<IdentityUserModel> userManager, RoleManager<IdentityRole> roleManager, DataContext dataContext, EmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dataContext = dataContext;
            _emailService = emailService;
        }
        // GET: Surveys
        public async Task<IActionResult> Index()
        {
            var surveysWithRoles = await (from survey in _dataContext.Surveys
                                          join role in _dataContext.Roles
                                          on survey.RoleId equals role.Id into roleGroup
                                          from role in roleGroup.DefaultIfEmpty()
                                          select new SurveyWithRoleViewModel
                                          {
                                              Id = survey.Id,
                                              Title = survey.Title,
                                              Description = survey.Description,
                                              CreatedDate = survey.CreatedDate,
                                              ExpiredDate = survey.ExpiredDate,
                                              RoleName = role != null ? role.Name : "No Role Assigned",
                                              IsActive = survey.IsActive
                                          })
                                      .AsNoTracking()
                                      .ToListAsync();

            return View(surveysWithRoles);
        }


        // GET: Surveys/Details/{id}
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var survey = await _dataContext.Surveys
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Options) // Include options for each question
                .Include(s => s.Questions)
                    .ThenInclude(q => q.QuestionType) // Include the question type
                .FirstOrDefaultAsync(m => m.Id == id);

            if (survey == null) return NotFound();

            // Map the survey entity to the view model
            var viewModel = new SurveyDetailsViewModel
            {
                Id = survey.Id,
                Title = survey.Title,
                Description = survey.Description,
                Questions = survey.Questions.Select(q => new QuestionWithOptionsDto
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText,
                    QuestionTypeId = q.QuestionType.Id,
                    QuestionTypeName = q.QuestionType.Name,
                    Options = q.Options.Select(o => new OptionDto
                    {
                        Id = o.Id,
                        OptionText = o.OptionText
                    }).ToList()
                }).ToList()
            };

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> ViewUsersForOption(int optionId)
        {
            // Get option details, including the question and survey it belongs to
            var optionDetails = await _dataContext.Options
                .Include(o => o.Question)
                .ThenInclude(q => q.Survey)
                .Where(o => o.Id == optionId)
                .Select(o => new
                {
                    OptionName = o.OptionText,
                    QuestionName = o.Question.QuestionText,
                    SurveyName = o.Question.Survey.Title,
                    SurveyId = o.Question.Survey.Id // Include the SurveyId
                })
                .FirstOrDefaultAsync();

            if (optionDetails == null)
            {
                ViewBag.Message = "Invalid option selected.";
                return View(new CompositeViewModel());
            }

            // Get user responses for the option
            var users = await _dataContext.ResponseDetails
                .Include(rd => rd.Response)
                .ThenInclude(r => r.User)
                .Where(rd => rd.OptionId == optionId)
                .Select(rd => new
                {
                    UserName = rd.Response.User != null ? rd.Response.User.UserName : "Unknown",
                    SubmittedDate = rd.Response.SubmittedDate
                })
                .ToListAsync();

            // If no users, return with a message
            if (!users.Any())
            {
                ViewBag.Message = "No users have selected this option.";
                return View(new CompositeViewModel
                {
                    SurveyName = optionDetails.SurveyName,
                    SurveyId = optionDetails.SurveyId, // Pass SurveyId to the view
                    QuestionName = optionDetails.QuestionName,
                    OptionName = optionDetails.OptionName,
                    UserResponses = new List<UserResponseViewModel>()
                });
            }

            // Map users to UserResponseViewModel
            var userResponses = users.Select(u => new UserResponseViewModel
            {
                UserName = u.UserName,
                SubmittedDate = u.SubmittedDate
            }).ToList();

            // Create CompositeViewModel
            var model = new CompositeViewModel
            {
                SurveyName = optionDetails.SurveyName,
                SurveyId = optionDetails.SurveyId, // Include SurveyId in the model
                QuestionName = optionDetails.QuestionName,
                OptionName = optionDetails.OptionName,
                UserResponses = userResponses
            };

            return View(model);
        }

        [HttpGet("Admin/Survey/ViewTextResponses/{questionId}")]
        public async Task<IActionResult> ViewTextResponses(int questionId)
        {
            var question = await _dataContext.Questions
                .Include(q => q.Survey)
                .FirstOrDefaultAsync(q => q.Id == questionId);

            if (question == null)
                return NotFound();

            var textResponses = await _dataContext.ResponseDetails
                .Include(rd => rd.Response)
                .ThenInclude(r => r.User)
                .Where(rd => rd.QuestionId == questionId && rd.AnswerText != null)
                .Select(rd => new TextResponseViewModel
                {
                    UserName = rd.Response.User != null ? rd.Response.User.UserName : "Anonymous",
                    TextAnswer = rd.AnswerText,
                    SubmittedDate = rd.Response.SubmittedDate
                }).ToListAsync();

            var viewModel = new TextResponsePageViewModel
            {
                SurveyId = question.SurveyId, // Include the survey ID
                SurveyTitle = question.Survey.Title,
                QuestionText = question.QuestionText,
                TextResponses = textResponses
            };

            return View(viewModel);
        }




        [HttpGet("Admin/Survey/SurveyResponses/{surveyId}")]
        public async Task<IActionResult> SurveyResponses(int surveyId)
        {
            var survey = await _dataContext.Surveys
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Options)
                .Include(s => s.Questions)
                    .ThenInclude(q => q.QuestionType)
                .FirstOrDefaultAsync(s => s.Id == surveyId);

            if (survey == null)
                return NotFound();

            var responses = await _dataContext.ResponseDetails
                .Include(rd => rd.Response)
                    .ThenInclude(r => r.User)
                .Where(rd => rd.Question.SurveyId == surveyId)
                .ToListAsync();

            var responseSummary = survey.Questions.Select(question => new QuestionResponseSummaryViewModel
            {
                QuestionId = question.Id,
                QuestionText = question.QuestionText,
                QuestionTypeName = question.QuestionType.Name,
                Options = question.Options.Select(option => new OptionResponseViewModel
                {
                    Id = option.Id,
                    OptionText = option.OptionText,
                    Count = responses.Count(r => r.OptionId == option.Id),
                    Percentage = responses.Count(r => r.QuestionId == question.Id) == 0
                        ? 0
                        : responses.Count(r => r.OptionId == option.Id) * 100.0 / responses.Count(r => r.QuestionId == question.Id)
                }).ToList()
            }).ToList();

            var viewModel = new SurveyResponseViewModel
            {
                SurveyId = survey.Id, // Pass the SurveyId
                SurveyTitle = survey.Title,
                ResponseSummary = responseSummary
            };

            return View(viewModel);
        }








        // GET: Surveys/Create
        public async Task<IActionResult> Create()
        {
            // Fetch roles for the dropdown
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name"); // Use "Id" for RoleId and "Name" for display

            return View();
        }

        // POST: Surveys/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SurveyModel survey, string selectedRoleId)
        {
            if (ModelState.IsValid)
            {
                survey.RoleId = selectedRoleId; // Assign the selected RoleId
                survey.CreatedDate = DateTime.Now;
                survey.IsActive = false; // Or set based on your logic
                survey.TotalResponses = 0;

                _dataContext.Add(survey);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction("Index"); // Redirect to the list of surveys or admin page
            }

            // Repopulate roles in case of validation error
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name", selectedRoleId);

            return View(survey);
        }




        // GET: Surveys/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null) return NotFound();

            var survey = await _dataContext.Surveys.FindAsync(id);
            if (survey == null) return NotFound();

            var isActiveOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Active", Value = "true" },
                new SelectListItem { Text = "Inactive", Value = "false" }
            };
            ViewBag.IsActive = new SelectList(isActiveOptions, "Value", "Text", survey.IsActive.ToString().ToLower());

            // Fetch roles for the dropdown
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name", survey.RoleId); // Preselect the current RoleId

            return View(survey);
        }

        // POST: Surveys/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SurveyModel survey, bool sendEmailToRoleUsers = false)
        {
            if (id != survey.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingSurvey = await _dataContext.Surveys.FindAsync(id);
                    if (existingSurvey == null) return NotFound();

                    // Update survey properties
                    existingSurvey.Title = survey.Title;
                    existingSurvey.Description = survey.Description;
                    existingSurvey.ExpiredDate = survey.ExpiredDate;
                    existingSurvey.RoleId = survey.RoleId;
                    existingSurvey.IsActive = survey.IsActive;

                    _dataContext.Update(existingSurvey);
                    await _dataContext.SaveChangesAsync();

                    // Send email if the option is enabled
                    if (sendEmailToRoleUsers)
                    {
                        var roleUsers = await (from user in _dataContext.Users
                                               join userRole in _dataContext.UserRoles on user.Id equals userRole.UserId
                                               where userRole.RoleId == survey.RoleId
                                               select user).ToListAsync();

                        foreach (var user in roleUsers)
                        {
                            var subject = "Survey Updated";
                            var body = $@"
                        Dear {user.UserName},<br><br>
                        The survey titled <strong>{survey.Title}</strong> has been updated. Please review it using the link below:<br><br>
                        <a href='https://localhost:7072/SurveyDetails/{survey.Id}'>View Survey</a><br><br>
                        Best regards,<br>Your Team
                            ";

                            await _emailService.SendEmailAsync(user.Email, subject, body);
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SurveyExists(survey.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(survey);
        }





        public async Task<IActionResult> Delete(int Id)
        {
            SurveyModel product = await _dataContext.Surveys.FindAsync(Id);

            _dataContext.Surveys.Remove(product);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool SurveyExists(int id)
        {
            return _dataContext.Surveys.Any(e => e.Id == id);
        }
    }
}
