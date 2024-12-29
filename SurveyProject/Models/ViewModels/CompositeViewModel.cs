namespace SurveyProject.Models.ViewModels
{
    public class CompositeViewModel
    {
        public string SurveyName { get; set; }
        public int SurveyId { get; set; } // Add SurveyId
        public string QuestionName { get; set; }
        public string OptionName { get; set; }
        public string QuestionTypeName { get; set; }
        public List<UserResponseViewModel> UserResponses { get; set; }
    }

}
