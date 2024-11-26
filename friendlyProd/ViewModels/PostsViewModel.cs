using friendly.Models;

namespace friendly.ViewModels
{
    // ViewModel for displaying posts in different views
    public class PostsViewModel
    {
        // A collection of posts to display
        public IEnumerable<Post> Posts { get; set; }

        public string? CurrentViewName { get; set; }

        // Constructor to initialize the PostsViewModel with a list of posts and the current view name
        public PostsViewModel(IEnumerable<Post> posts, string? currentViewName)
        {
            Posts = posts;
            CurrentViewName = currentViewName;
        }
    }
}
