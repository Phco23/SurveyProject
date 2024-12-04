using System.ComponentModel.DataAnnotations;

namespace SurveyProject.Models
{
    public class QuestionTypeModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } 
    }
}
