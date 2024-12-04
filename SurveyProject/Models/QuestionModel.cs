using System.ComponentModel.DataAnnotations;

namespace SurveyProject.Models
{
    public class QuestionModel
    {
        [Key]
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public string QuestionText {  get; set; }
        public int QuestionTypeId { get; set; }
        public SurveyModel Survey { get; set; }
        public QuestionTypeModel QuestionType { get; set; }
    }

}
