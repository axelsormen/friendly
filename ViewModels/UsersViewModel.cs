using friendly.Models;

namespace friendly.ViewModels
{
    public class UsersViewModel
    {
        public User? User { get; set; }  // Single user object
        public IEnumerable<Post>? Posts { get; set; }  // User's posts
        public string? CurrentViewName { get; set; }

        public UsersViewModel(User? user, IEnumerable<Post>? posts, string? currentViewName)
        {
            User = user;
            Posts = posts;
            CurrentViewName = currentViewName;
        }
    }
}
