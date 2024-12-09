using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyProject.Models;
using SurveyProject.Repository;

namespace SurveyProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FAQController : Controller
    {
        private readonly DataContext _context;
        public FAQController (DataContext context)
        {
            _context = context;
        }

        // GET: FAQ
        public async Task<IActionResult> Index ()
        {
            return View(await _context.FAQs.ToListAsync());
        }

        // GET: CREATE
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FAQ faq)
        {
            if (ModelState.IsValid)
            {
                _context.FAQs.Add(faq);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(faq);
        }
        // GET: FAQ/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faq = await _context.FAQs.FindAsync(id);
            if (faq == null)
            {
                return NotFound();
            }
            return View(faq);
        }

        // POST: FAQ/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FAQ faq)
        {
            if (id != faq.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                    _context.Update(faq);
                    await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
            }
            return View(faq);
        }

        // GET: FAQ/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var faq = await _context.FAQs.FirstOrDefaultAsync(m => m.Id == id);
            if (faq == null) return NotFound();
            return View(faq);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var faq = await _context.FAQs.FindAsync(id);
            _context.FAQs.Remove(faq);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
    }
}