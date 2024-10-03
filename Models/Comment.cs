using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace friendly.Models
{
    public class Comment
    {
        public int CommentId { get; set; }

        [StringLength(200)] 
        public string CommentText { get; set; }

        public string CommentDate { get; set; }

        [ValidateNever]
        public virtual User User { get; set; }

        public int UserId { get; set; }

        [ValidateNever]
        public virtual Post Post { get; set; }

        public int PostId { get; set; }
    }
}
