using friendly.Models;

namespace friendly.ViewModels
{
    public class UsersViewModel
    {
        public IEnumerable<User>? Users { get; set; }  // List of Users for the Table view
        public User? User { get; set; }  // Single User for the Details view
        public IEnumerable<Post>? Posts { get; set; }  // User's posts
        public string? CurrentViewName { get; set; }

        // Constructor for multiple users
        public UsersViewModel(IEnumerable<User>? users, string? currentViewName)
        {
            Users = users;
            CurrentViewName = currentViewName;
        }

        // Constructor for a single user with posts
        public UsersViewModel(User? user, IEnumerable<Post>? posts, string? currentViewName)
        {
            User = user;
            Posts = posts;
            CurrentViewName = currentViewName;
        }
    }
}
