using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyProject.Models;
using SurveyProject.Repository;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;


namespace SurveyProject.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize]
    public class FeedbackController : Controller
	{
        private readonly DataContext _context;
        


        public FeedbackController(DataContext context)
		{
			_context = context;

		}
		[HttpGet]
		public IActionResult Index()
		{
			var feedbacks = _context.Feedbacks.ToList(); 
			return View(feedbacks);
		}

        public IActionResult ManageFeedback()
        {
            var feedbacks = _context.Feedbacks
                .OrderByDescending(f => f.SubmittedAt)
                .ToList();

            Console.WriteLine($"Total feedbacks retrieved: {feedbacks.Count}");
            foreach (var feedback in feedbacks)
            {
                Console.WriteLine($"ID: {feedback.Id}, Content: {feedback.Content}, SubmittedAt: {feedback.SubmittedAt}");
            }

            return View(feedbacks);
        }


        [HttpPost]
        public async Task<IActionResult> SubmitFeedback(FeedbackModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "You must be logged in to submit feedback.";
                return RedirectToAction("Login", "Account"); 
            }

            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;

                //var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                model.SubmittedAt = DateTime.Now; 
                model.IsReviewed = false;
                model.UserId = userId;
                model.UserName = userName;
                //model.Email = userEmail;

                _context.Feedbacks.Add(model);
                await _context.SaveChangesAsync();

               

                TempData["SuccessMessage"] = "Feedback submitted successfully!";
                return Redirect("/Home/ResponseFeedback"); 
            }

            TempData["ErrorMessage"] = "Failed to submit feedback. Please try again.";
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
		public IActionResult SubmitFeedback()
		{
			return RedirectToAction("Index");
		}


        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                TempData["ErrorMessage"] = "Feedback not found.";
                return RedirectToAction("ManageFeedback");
            }

            return View(feedback);
        }

        [HttpPost, ActionName("DeleteFeedback")]    
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                TempData["ErrorMessage"] = "Feedback not found.";
                return RedirectToAction("ManageFeedback");
            }

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Feedback deleted successfully.";
            return RedirectToAction("ManageFeedback");
        }


        public IActionResult Detail(int id)
        {
            var feedback = _context.Feedbacks.FirstOrDefault(f => f.Id == id);
            if (feedback == null)
            {
                return NotFound();
            }
            return View(feedback);
        }

        [HttpPost]
        public IActionResult SubmitResponse(int id, string response)
        {

            TempData["SuccessMessage"] = "Response submitted successfully!";

            return RedirectToAction("ManageFeedback");
        }


        //[HttpPost]
        //public IActionResult SubmitResponse(int id, string response)
        //{
        //    var feedback = _context.Feedbacks.FirstOrDefault(f => f.Id == id);
        //    if (feedback == null)
        //    {
        //        return NotFound();
        //    }

        //    feedback.Response = response;
        //    _context.Feedbacks.Update(feedback);
        //    _context.SaveChanges();

        //    TempData["SuccessMessage"] = "Phản hồi đã được gửi thành công!";
        //    return RedirectToAction("Detail", new { id = id });
        //}




    }
}
