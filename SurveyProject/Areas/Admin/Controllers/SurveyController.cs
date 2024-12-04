using Microsoft.AspNetCore.Mvc;

namespace SurveyProject.Areas.Admin.Controllers
{
    public class SurveyController : Controller
    {
        [Area("Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
