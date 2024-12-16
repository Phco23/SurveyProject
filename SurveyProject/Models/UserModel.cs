using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SurveyProject.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter UserName")]

        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter Email"), EmailAddress]

        public string Email { get; set; }

        [DataType(DataType.Password), Required(ErrorMessage = "Please enter Password")]

        public string Password { get; set; }
        [NotMapped]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
