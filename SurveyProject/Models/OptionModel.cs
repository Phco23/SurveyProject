using System.ComponentModel.DataAnnotations;

namespace SurveyProject.Models
{
    public class OptionModel
    {
        [Key]
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string OptionText { get; set; }
        public int Score { get; set; }
        public QuestionModel Question { get; set; }
    }
}
