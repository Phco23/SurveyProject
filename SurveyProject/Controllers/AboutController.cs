using Microsoft.AspNetCore.Mvc;

namespace SurveyProject.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
