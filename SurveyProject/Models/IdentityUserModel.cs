using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SurveyProject.Models
{
    public class IdentityUserModel : IdentityUser
    {
        public string RoleId { get; set; }
        public bool IsApproved { get; set; } = false;
       
    }
}