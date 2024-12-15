using System.ComponentModel.DataAnnotations;

namespace SurveyProject.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "username is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
        public bool RememberLogin { get; set; }

    }

}
