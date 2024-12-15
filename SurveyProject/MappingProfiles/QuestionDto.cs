namespace SurveyProject.MappingProfiles
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public int QuestionTypeId { get; set; }
        public int SurveyId { get; set; }

    }
}
