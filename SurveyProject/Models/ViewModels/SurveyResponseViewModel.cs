namespace SurveyProject.Models.ViewModels
{
    public class SurveyResponseViewModel
    {
        public int SurveyId { get; set; } 
        public string SurveyTitle { get; set; }
        public List<QuestionResponseSummaryViewModel> ResponseSummary { get; set; }
    }
}
