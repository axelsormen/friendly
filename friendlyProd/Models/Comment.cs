using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace friendly.Models
{
    public class Comment
    {
        public int CommentId { get; set; }

        [Required(ErrorMessage = "Comment text is required.")]
        [MinLength(1, ErrorMessage = "Comment must be at least 1 character long.")]
        [MaxLength(200, ErrorMessage = "Comment cannot exceed 200 characters.")]
        public string? CommentText { get; set; }

        public string? CommentDate { get; set; }

        [ValidateNever]
        public virtual User? User { get; set; }

        public string? UserId { get; set; }

        [ValidateNever]
        public virtual Post? Post { get; set; }

        public int PostId { get; set; }
    }
}
