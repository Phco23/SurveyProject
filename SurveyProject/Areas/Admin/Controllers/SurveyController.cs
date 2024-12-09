using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyProject.Models;
using SurveyProject.Repository;

namespace SurveyProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SurveyController : Controller
    {
        private readonly DataContext _dataContext;
        public SurveyController(DataContext context)
        {
            _dataContext = context;
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

        // GET: Surveys/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Surveys/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SurveyModel survey)
        {
			if (ModelState.IsValid)
			{
				survey.CreatedDate = DateTime.Now;
				survey.IsActive = true;
				survey.TotalResponses = 0;

				_dataContext.Add(survey);
				await _dataContext.SaveChangesAsync();
				return Redirect("/admin");
			}
			return View(survey);
		}

        // GET: Surveys/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var survey = await _dataContext.Surveys.FindAsync(id);
            if (survey == null) return NotFound();

            return View(survey);
        }

        // POST: Surveys/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SurveyModel survey)
        {
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
