using System.ComponentModel.DataAnnotations;

namespace SurveyProject.Models
{
    public class FAQModel
    {
        [Key]
        public int Id {get; set;}
        [Required]
        public string Tittle {get; set;}
        [Required]
        public string Content {get; set;}
    }
}