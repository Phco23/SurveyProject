namespace SurveyProject.Models.ViewModels
{
    public class SurveyResponseViewModel
    {
        public SurveyModel Survey { get; set; }
        public List<ResponseModel> Responses { get; set; } // User submissions
        public List<ResponseDetailsModel> ResponseDetails { get; set; } // Detailed answers
    }
}
