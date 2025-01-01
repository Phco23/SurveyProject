namespace SurveyProject.Models.ViewModels
{
    public class UserResponseViewModel
    {
        public string UserName { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string TextAnswer { get; set; } // Add property for text answer
    }

}
