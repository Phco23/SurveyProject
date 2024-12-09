using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyProject.Repository;

namespace SurveyProject.Controllers
{
    public class FAQController : Controller
    {
        private readonly DataContext _context;
        public FAQController (DataContext context)
        {
            _context = context;
        }

        // GET: FAQ
        public async Task<IActionResult> Index()
        {
            return View(await _context.FAQs.ToListAsync());
        }

    }
}