using Microsoft.AspNetCore.Identity;

namespace SurveyProject.Models
{
    public class IdentityUserModel : IdentityUser
    {
        public string RoleId { get; set; }
        public bool IsApproved { get; set; } = false;
    }
}
