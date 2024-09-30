using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace friendly.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; } 
        public string Username { get; set; } 
        public string Email { get; set; }
        public string? ProfileImageUrl { get; set; }
        
        [ValidateNever]
        public virtual List<Post>? Posts { get; set; }
    }
}
