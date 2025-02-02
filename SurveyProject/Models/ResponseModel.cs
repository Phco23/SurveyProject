﻿using System.ComponentModel.DataAnnotations;

namespace SurveyProject.Models
{
    public class ResponseModel
    {
        [Key]
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public string UserId { get; set; }
        public DateTime SubmittedDate { get; set; }
        public SurveyModel Survey { get; set; }
        public IdentityUserModel User { get; set; }
    }
}
