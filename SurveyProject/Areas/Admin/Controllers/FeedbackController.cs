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
		public FeedbackController(	 DataContext context)
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

                //var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                //if (!string.IsNullOrEmpty(userEmail))
                //{
                //    await SendFeedbackEmail(userEmail, model.Content);
                //}
                //if (string.IsNullOrEmpty(userEmail))
                //{
                //    TempData["ErrorMessage"] = "Unable to retrieve user email. Please check your account.";
                //}

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

            // Truyền thông tin phản hồi đến view để hiển thị cho người dùng
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


        //[HttpGet]
        //public IActionResult ThankYou()
        //{
        //    return View();
        //}

        //private async Task SendFeedbackEmail(string recipientEmail, string feedbackContent)
        //{
        //    var smtpClient = new SmtpClient("smtp.gmail.com")
        //    {
        //        Port = 587,
        //        Credentials = new NetworkCredential("lequangnam2712005@gmail.com", "mtpl sekm kvii oaf"),
        //        EnableSsl = true,
        //    };

        //    var mailMessage = new MailMessage
        //    {
        //        From = new MailAddress("lequangnam27012005@gmail.com"),
        //        Subject = "Thank you for your feedback!",
        //        Body = $"<p>Dear user,</p><p>Thank you for your feedback:</p><p>{feedbackContent}</p><p>We will respond to you as soon as possible.</p><p>Best regards,</p><p>Your Team</p>",
        //        IsBodyHtml = true,
        //    };

        //    mailMessage.To.Add(recipientEmail);

        //    await smtpClient.SendMailAsync(mailMessage);
        //}


    }
}
