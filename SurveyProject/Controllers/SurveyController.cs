using Microsoft.AspNetCore.Mvc;

namespace SurveyProject.Controllers
{
    public class SurveyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ListSurvey()
        {
            return View();
        }
    }
}
