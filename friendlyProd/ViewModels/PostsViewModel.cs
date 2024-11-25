using friendly.Models;

namespace friendly.ViewModels
{
    public class PostsViewModel
    {
        public IEnumerable<Post> Posts { get; set; }
        public string? CurrentViewName { get; set; }

        public PostsViewModel(IEnumerable<Post> posts, string? currentViewName)
        {
            Posts = posts;
            CurrentViewName = currentViewName;
        }
    }
}
