using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace friendly.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
    
        [ValidateNever]
        public string? PostImagePath { get; set; }  // Store the image file path

        [Required]
        [StringLength(200)] 
        public string? Caption { get; set; }

        [ValidateNever]
        public string? PostDate { get; set; } 

        // Reference to the IdentityUser's Id
        [ValidateNever]
        public string? UserId { get; set; }

        // Navigation property to IdentityUser
        [ValidateNever]
        public virtual User? User { get; set; }

        // Navigation property for the comments
        [ValidateNever]
        public virtual List<Comment>? Comments { get; set; }
    }
}
