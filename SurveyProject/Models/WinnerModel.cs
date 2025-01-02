using Microsoft.AspNetCore.Identity;
using System;

namespace SurveyProject.Models
{
    public class WinnerModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RollNumber { get; set; }
        public DateTime AwardedOn { get; set; }

        public int ContestId { get; set; }
        public ContestModel Contest { get; set; }
    }
}
