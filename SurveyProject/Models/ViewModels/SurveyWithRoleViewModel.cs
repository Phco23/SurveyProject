namespace SurveyProject.Models.ViewModels
{
    public class SurveyWithRoleViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ExpiredDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
    }
}
