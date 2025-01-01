namespace SurveyProject.Models.ViewModels
{
    public class TextResponsePageViewModel
    {
        public int SurveyId { get; set; } // For navigating back
        public string SurveyTitle { get; set; }
        public string QuestionText { get; set; }
        public List<TextResponseViewModel> TextResponses { get; set; }
    }
}
