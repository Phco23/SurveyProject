using System.ComponentModel.DataAnnotations;

namespace SurveyProject.Models
{
    public class ResponseDetailsModel
    {
        [Key]
        public int Id { get; set; } 
        public int ResponseId { get; set; }
        public int QuestionId { get; set; }
        public string AnswerText { get; set; }
        public int OptionId { get; set; }
        public ResponseModel Response {  get; set; }
        public QuestionModel Question { get; set; }
        public OptionModel Option { get; set; }
    }
}
