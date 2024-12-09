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
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public int TotalResponses { get; set; }
        public bool IsActive { get; set; }

        public ICollection<QuestionModel>? Questions { get; set; }
    }
}
