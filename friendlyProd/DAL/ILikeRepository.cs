using System.Threading.Tasks;
using friendly.Models;

namespace friendly.DAL
{
    public interface ILikeRepository
    {
        // Asynchronously creates a new like for a post by a user
        Task<bool> Create(Like like);

        // Asynchronously deletes a like based on the post ID and user ID
        Task<bool> DeleteByPostAndUser(int postId, string userId);

        // Asynchronously retrieves the total count of likes for a specific post
        Task<int> GetLikesCount(int postId); // New method to get likes count
    }
}
