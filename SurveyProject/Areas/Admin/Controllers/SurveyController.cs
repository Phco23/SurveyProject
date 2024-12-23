using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        public SurveyController(UserManager<IdentityUserModel> userManager, RoleManager<IdentityRole> roleManager, DataContext dataContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dataContext = dataContext;
        }
        // GET: Surveys
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Surveys.ToListAsync());
        }
        // GET: Surveys/Details/{id}
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var survey = await _dataContext.Surveys
                .Include(s => s.Questions) 
                .ThenInclude(q => q.QuestionType) 
                .FirstOrDefaultAsync(m => m.Id == id);

            if (survey == null) return NotFound();

            return View(survey);
        }

        [HttpGet("Admin/Survey/SurveyResponses/{surveyId}")]
        public async Task<IActionResult> SurveyResponses(int surveyId)
        {
            var survey = await _dataContext.Surveys
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Options)
                .Include(s => s.Questions)
                    .ThenInclude(q => q.QuestionType) // Include QuestionType
                .FirstOrDefaultAsync(s => s.Id == surveyId);

            if (survey == null) return NotFound();

            var responses = await _dataContext.ResponseDetails
                .Include(rd => rd.Response)
                    .ThenInclude(r => r.User)
                .Where(rd => rd.Question.SurveyId == surveyId)
                .ToListAsync();

            var responseSummary = survey.Questions.Select(question => new
            {
                QuestionText = question.QuestionText,
                QuestionTypeId = question.QuestionTypeId,
                QuestionTypeName = question.QuestionType.Name, // Include type name
                Options = question.Options.Select(option => new
                {
                    OptionText = option.OptionText,
                    Count = responses.Count(r => r.OptionId == option.Id),
                    Percentage = responses.Count(r => r.OptionId == option.Id) * 100.0 / responses.Count(r => r.QuestionId == question.Id)
                }).ToList(),
                UserResponses = responses
                    .Where(r => r.QuestionId == question.Id)
                    .Select(r => new
                    {
                        UserName = r.Response.User.UserName,
                        SelectedOption = r.Option?.OptionText ?? r.AnswerText
                    }).ToList()
            }).ToList();

            ViewBag.SurveyTitle = survey.Title;
            ViewBag.ResponseSummary = responseSummary;

            return View();
        }




        // GET: Surveys/Create
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Name", "Name");

            return View();
        }

        // POST: Surveys/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SurveyModel survey, string assignedRole)
        {
            if (ModelState.IsValid)
            {
                survey.CreatedDate = DateTime.Now;
                survey.IsActive = false;
                survey.TotalResponses = 0;
                survey.RoleId = assignedRole; // Assign the selected role

                _dataContext.Add(survey);
                await _dataContext.SaveChangesAsync();
                return Redirect("/admin");
            }

            // Repopulate roles in case of validation error
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Name", "Name", assignedRole);

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

            return View(survey);
        }

        // POST: Surveys/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SurveyModel survey)
        {
            var isActiveOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Active", Value = "true" },
                new SelectListItem { Text = "Inactive", Value = "false" }
            };
            ViewBag.IsActive = new SelectList(isActiveOptions, "Value", "Text", survey.IsActive.ToString().ToLower());

            if (id != survey.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _dataContext.Update(survey);
                    await _dataContext.SaveChangesAsync();
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
