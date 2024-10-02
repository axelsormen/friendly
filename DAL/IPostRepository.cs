using friendly.Models;

namespace friendly.DAL;

public interface IPostRepository
{
    Task<IEnumerable<Post>?> GetAll();
    Task<IEnumerable<Post>?> GetPostsByUserId(int userId); // Add this line
    Task<Post?> GetPostById(int id);
    Task<bool> Create(Post post);
    Task<bool> Update(Post post);
    Task<bool> Delete(int id);
}