using Microsoft.AspNetCore.Identity;
using System;

namespace SurveyProject.Models
{
    public class WinnerModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public int ContestId { get; set; }
    }
}
