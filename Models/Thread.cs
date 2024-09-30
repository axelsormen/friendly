using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace friendly.Models
{
    public class Thread
    {
        public int ThreadId { get; set; }
        public string ThreadText { get; set; }
        public string ThreadDate { get; set; }

        [ValidateNever]
        public virtual User User { get; set; } // Changed from Users to User
        public int UserId { get; set; }
        
        [ValidateNever]
        public virtual Comment Comment { get; set; } // Changed from Comments to Comment
        public int CommentId { get; set; }
    }
}
