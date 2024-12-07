using System.ComponentModel.DataAnnotations;

namespace SurveyProject.Models
{
    public class QuestionModel
    {
        [Key]
        public int Id { get; set; }
        public int SurveyId { get; set; }
        [Required]
        public string QuestionText {  get; set; }
        [Required]
        public int QuestionTypeId { get; set; }
        public SurveyModel Survey { get; set; }
        public QuestionTypeModel QuestionType { get; set; }

        public ICollection<OptionModel>? Options { get; set; }
    }

}
