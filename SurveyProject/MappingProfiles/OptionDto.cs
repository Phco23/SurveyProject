namespace SurveyProject.MappingProfiles
{
    public class OptionDto
    {
        public int Id { get; set; }
        public string OptionText { get; set; }
        public int Score { get; set; }
        public int QuestionId { get; set; }
    }
}
