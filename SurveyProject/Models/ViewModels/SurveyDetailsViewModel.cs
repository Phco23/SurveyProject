using SurveyProject.MappingProfiles;

namespace SurveyProject.Models.ViewModels
{
    public class SurveyDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<QuestionWithOptionsDto> Questions { get; set; } = new List<QuestionWithOptionsDto>();
    }
}
