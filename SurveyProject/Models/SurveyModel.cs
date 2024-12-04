using System.ComponentModel.DataAnnotations;

namespace SurveyProject.Models
{
    public class SurveyModel
    {
        [Key]   
        public int Id { get; set; }
        public string RoleId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsActive { get; set; }
    }
}
