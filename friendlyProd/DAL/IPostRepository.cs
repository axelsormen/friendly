using friendly.Models;

namespace friendly.DAL
{
    public interface IPostRepository
    {
        // Asynchronously retrieves all posts from the database
        Task<IEnumerable<Post>?> GetAll();

        // Asynchronously retrieves all posts by a specific user based on their user ID
        Task<IEnumerable<Post>?> GetPostsByUserId(string userId);

        // Asynchronously retrieves a single post by its ID
        Task<Post?> GetPostById(int id);

        // Asynchronously creates a new post
        Task<bool> Create(Post post);

        // Asynchronously updates an existing post
        Task<bool> Update(Post post);

        // Asynchronously deletes a post based on its ID
        Task<bool> Delete(int id);
    }
}
