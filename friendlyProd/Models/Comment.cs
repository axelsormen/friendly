using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace friendly.Models
{
    public class Comment
    {
        public int CommentId { get; set; }

        // The actual text of the comment
        [Required(ErrorMessage = "Comment text is required.")]
        [MinLength(1, ErrorMessage = "Comment must be at least 1 character long.")]
        [MaxLength(200, ErrorMessage = "Comment cannot exceed 200 characters.")]
        public string? CommentText { get; set; }

        // The date when the comment was posted
        public string? CommentDate { get; set; }

        // Navigation property for the user who posted the comment
        [ValidateNever]
        public virtual User? User { get; set; }

        // The UserId representing the user who posted the comment
        public string? UserId { get; set; }

        // Navigation property for the post the comment is related to
        [ValidateNever]
        public virtual Post? Post { get; set; }

        // Foreign key representing the post that this comment belongs to
        public int PostId { get; set; }
    }
}
