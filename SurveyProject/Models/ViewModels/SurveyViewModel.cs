namespace SurveyProject.Models.ViewModels
{
    public class SurveyViewModel
    {
        public int SurveyId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ExpiredDate { get; set; }
        public bool IsActive { get; set; }

        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
    }
}
