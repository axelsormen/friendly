using friendly.Models;

namespace friendly.DAL;

public interface IPostRepository
{
    Task<IEnumerable<Post>> GetAll();
    Task<IEnumerable<Post>> GetPostsByUserId(int userId); // Add this line
    Task<Post?> GetPostById(int id);
    Task Create(Post post);
    Task Update(Post post);
    Task<bool> Delete(int id);
}