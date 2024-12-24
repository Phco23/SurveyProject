using Microsoft.AspNetCore.Mvc;

namespace SurveyProject.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
