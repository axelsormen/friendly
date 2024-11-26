using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace friendly.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
    
        // The file path for the image to the post
        [ValidateNever]
        public string? PostImagePath { get; set; }  

        // Caption of the post
        [Required]
        [StringLength(200)]
        public string? Caption { get; set; }

        // The date the post was created
        [ValidateNever]
        public string? PostDate { get; set; }

        // Foreign key to the user who created the post
        [ValidateNever]
        public string? UserId { get; set; }

        // Navigation property to the user who created the post
        [ValidateNever]
        public virtual User? User { get; set; }

        // Navigation property for the list of comments on this post 
        [ValidateNever]
        public virtual List<Comment>? Comments { get; set; }
        
        // Navigation property for the list of likes for this post 
        [ValidateNever]
        public virtual List<Like>? Likes { get; set; }
    }
}