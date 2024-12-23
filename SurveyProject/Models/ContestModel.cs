using Microsoft.AspNetCore.Identity;
using System.Reflection;

namespace SurveyProject.Models
{
    public class ContestModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
