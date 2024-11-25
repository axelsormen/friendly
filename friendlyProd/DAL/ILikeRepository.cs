using System.Threading.Tasks;
using friendly.Models;

namespace friendly.DAL
{
    public interface ILikeRepository
    {
        Task<bool> Create(Like like);
        Task<bool> DeleteByPostAndUser(int postId, string userId);
        Task<int> GetLikesCount(int postId); // New method to get likes count
    }
}
