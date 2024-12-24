namespace SurveyProject.MappingProfiles
{
    public class QuestionWithOptionsDto
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public int QuestionTypeId { get; set; }
        public List<OptionDto> Options { get; set; } = new List<OptionDto>();
    }
}
