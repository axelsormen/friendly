using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace friendly.Models
{
    public class User : IdentityUser
    {
        // First name of the user
        public string? FirstName { get; set; }

        // Last name of the user
        public string? LastName { get; set; }

        // URL to the user's profile image
        public string? ProfileImageUrl { get; set; }

        // Navigation property for the list of posts created by the user
        public virtual List<Post>? Posts { get; set; }
    }
}
