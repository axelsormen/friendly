using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace friendly.Models
{
    public class Like
    {
        public int LikeId { get; set; } // Primærnøkkel for Like

        [Required(ErrorMessage = "Post ID is required.")]
        public int PostId { get; set; } // Fremmednøkkel til Post

        [Required(ErrorMessage = "User ID is required.")]
        [MaxLength(450)] // Typisk lengde for UserId
        public string? UserId { get; set; } // Fremmednøkkel til User

        [ValidateNever]
        public virtual Post? Post { get; set; } // Navigasjonspropertie til Post
    }
}


