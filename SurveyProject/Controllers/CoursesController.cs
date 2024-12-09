using Microsoft.AspNetCore.Mvc;

namespace SurveyProject.Controllers
{
    public class CoursesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
