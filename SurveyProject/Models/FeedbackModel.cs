    using System;
using System.ComponentModel.DataAnnotations;

namespace SurveyProject.Models
{
    public class FeedbackModel
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        [StringLength(500, ErrorMessage = "phan hoi kh dc dai qua 500")]
        public string Content { get; set; } 

        public DateTime SubmittedAt { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public string UserId { get; set; } 

        public bool IsReviewed { get; set; } = false;
        //public string Response { get; set; }
    }
}
