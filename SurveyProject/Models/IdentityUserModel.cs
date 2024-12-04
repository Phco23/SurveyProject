using Microsoft.AspNetCore.Identity;

namespace SurveyProject.Models
{
    public class IdentityUserModel : IdentityUser
    {
        public string RoleId { get; set; }
    }
}
