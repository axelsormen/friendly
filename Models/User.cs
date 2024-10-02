using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace friendly.Models
{
    public class User
    {
        public int UserId { get; set; }

        [RegularExpression(@"[a-zA-ZæøåÆØÅ -]{2,20}", ErrorMessage = "The Name must be letters and between 2 to 20 characters.")]
        [Display(Name = "Name")]
        public string Name { get; set; } 

        [RegularExpression(@"[0-9a-zA-Z. \-]{2,20}", ErrorMessage = "The Username must be numbers or letters and between 2 to 20 characters.")]
        [Display(Name = "Username")]
        public string Username { get; set; } 

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        public string? ProfileImageUrl { get; set; }
        
        [ValidateNever]
        public virtual List<Post>? Posts { get; set; }
    }
}
