namespace SurveyProject.Models.ViewModels
{
    public class QuestionResponseSummaryViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionTypeName { get; set; }
        public List<OptionResponseViewModel> Options { get; set; }
    }
}
