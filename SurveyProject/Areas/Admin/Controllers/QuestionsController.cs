using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SurveyProject.MappingProfiles;
using SurveyProject.Models;
using SurveyProject.Repository;

namespace SurveyProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QuestionsController : Controller
    {
        private readonly DataContext _context;

        public QuestionsController(DataContext context)
        {
            _context = context;
        }

        // GET: Questions
        public async Task<IActionResult> Index()
        {
            var questions = _context.Questions
                .Include(q => q.Survey)
                .Include(q => q.QuestionType);
            return View(await questions.ToListAsync());
        }

        // GET: Questions/Details/5
        public IActionResult Details(int id)
        {
            var question = _context.Questions
                .Include(q => q.Options) // Load related options
                .FirstOrDefault(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            var questionDto = new QuestionWithOptionsDto
            {
                Id = question.Id,
                QuestionText = question.QuestionText,
                QuestionTypeId = question.QuestionTypeId,
                Options = question.Options.Select(o => new OptionDto
                {
                    Id = o.Id,
                    OptionText = o.OptionText
                }).ToList()
            };

            return View(questionDto);
        }

        public IActionResult AddOption(int questionId)
        {
            ViewBag.QuestionId = questionId; // Pass QuestionId to the view
            return View(new OptionDto());
        }

        [HttpPost]
        public IActionResult AddOption(int questionId, OptionDto optionDto)
        {
            if (ModelState.IsValid)
            {
                var option = new OptionModel
                {
                    QuestionId = questionId,
                    OptionText = optionDto.OptionText
                };

                _context.Options.Add(option);
                _context.SaveChanges();

                return RedirectToAction("Details", new { id = questionId });
            }

            return View(optionDto);
        }

        // GET: Questions/Create
        public IActionResult Create(int? surveyId)
        {
            if (surveyId == null)
            {
                return NotFound();
            }

            // Populate dropdown list for question types
            ViewData["QuestionTypeId"] = new SelectList(_context.QuestionTypes, "Id", "Name");

            // Pass SurveyId to the view for binding    
            var model = new QuestionDto
            {
                SurveyId = surveyId.Value
            };

            return View(model);
        }

        // POST: Questions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionDto questionDto)
        {
            if (ModelState.IsValid)
            {
                var question = new QuestionModel
                {
                    QuestionText = questionDto.QuestionText,
                    QuestionTypeId = questionDto.QuestionTypeId,
                    SurveyId = questionDto.SurveyId
                };

                _context.Questions.Add(question);
                await _context.SaveChangesAsync();

                // Redirect after adding the question
                return RedirectToAction("Details", "Survey", new { id = question.SurveyId });
            }

            // Handle validation errors and pass the relevant data
            ViewData["QuestionTypeId"] = new SelectList(_context.QuestionTypes, "Id", "Name", questionDto.QuestionTypeId);
            return View(questionDto);  // Returning the DTO to the view
        }


        // GET: Questions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound();

            ViewData["QuestionTypeId"] = new SelectList(_context.QuestionTypes, "Id", "Name", question.QuestionTypeId);

            var model = new QuestionDto
            {
                SurveyId = id.Value
            };

            return View(question);
        }

        // POST: Questions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, QuestionDto questionDto)
        {
            if (id != questionDto.SurveyId) return NotFound();

            if (ModelState.IsValid)
            {

                var question = new QuestionModel
                {
                    QuestionText = questionDto.QuestionText,
                    QuestionTypeId = questionDto.QuestionTypeId,
                    SurveyId = questionDto.SurveyId
                };

                _context.Update(question);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Surveys", new { id = questionDto.SurveyId });
            }

            ViewData["QuestionTypeId"] = new SelectList(_context.QuestionTypes, "Id", "Name", questionDto.QuestionTypeId);
            return View(questionDto);
        }

        // GET: Questions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var question = await _context.Questions
                .Include(q => q.Survey)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (question == null) return NotFound();

            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question != null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Surveys", new { id = question?.SurveyId });
        }
    }

}
