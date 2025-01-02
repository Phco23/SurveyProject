using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyProject.Models;
using SurveyProject.Repository;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using eLearning.Repository;


namespace SurveyProject.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize]
    public class FeedbackController : Controller
	{
        private readonly DataContext _context;
        private readonly EmailService _emailService;


        public FeedbackController(DataContext context, EmailService emailService)
		{
			_context = context;
            _emailService = emailService;

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
        public async Task<IActionResult> SubmitResponse(int id, string response)
        {
            // Retrieve the feedback by ID
            var feedback = _context.Feedbacks.FirstOrDefault(f => f.Id == id);
            if (feedback == null)
            {
                return NotFound();
            }

            // Save the admin's response
            feedback.Response = response;
            feedback.IsReviewed = true;
            _context.SaveChanges();

            // Send an email to notify the user
            var subject = "Response to Your Feedback";
            var body = $@"
        <div style='font-family: Arial, sans-serif; background-color: #f4f7fa; padding: 20px;'>
            <div class='container' style='max-width: 600px; background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1); padding: 30px;'>
                <h2 style='color: #007bff; text-align: center;'>Response to Your Feedback</h2>
                <p style='font-size: 16px; color: #555;'>Dear {feedback.UserName},</p>

                <p style='font-size: 16px; color: #555;'>Thank you for sharing your feedback with us.</p>

                <p style='font-size: 16px; color: #555;'>Our response to your feedback:</p>
                <blockquote style='font-size: 16px; color: #333; background-color: #f8f9fa; padding: 15px; border-left: 4px solid #007bff; margin: 10px 0;'>
                    {response}
                </blockquote>

                <p style='font-size: 16px; color: #555;'>We hope this addresses your concerns. If you have further questions or feedback, feel free to contact us.</p>

                <div style='text-align: center; margin: 20px 0;'>
                    <a href='https://localhost:7072/Contact' style='background-color: #007bff; color: white; text-decoration: none; padding: 10px 20px; border-radius: 5px;'>Contact Support</a>
                </div>

                <p style='font-size: 16px; color: #555;'>Best regards,</p>
                <p style='font-size: 16px; color: #555; font-weight: bold;'>The eLearning Team</p>
            </div>
        </div>
    ";

            // Send email using your EmailService
            await _emailService.SendEmailAsync(feedback.Email, subject, body);

            return RedirectToAction("Detail", new { id });
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

                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                model.SubmittedAt = DateTime.Now;
                model.IsReviewed = false;
                model.UserId = userId;
                model.UserName = userName;
                model.Email = userEmail;

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



    }
}
