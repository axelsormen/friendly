using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace friendly.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string CommentText { get; set; }
        public string CommentDate { get; set; }

        [ValidateNever]
        public virtual User User { get; set; } // Changed from Users to User
        public int UserId { get; set; }

        [ValidateNever]
        public virtual Post Post { get; set; } // Changed from Posts to Post
        public int PostId { get; set; }

        [ValidateNever]
        public virtual List<Thread> Threads { get; set; } // This should already be fine
    }
}
