using Microsoft.AspNetCore.Identity;
using System.Reflection;

namespace SurveyProject.Models
{
    public class ContestModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        public ICollection<WinnerModel> Winners { get; set; }
    }
}
