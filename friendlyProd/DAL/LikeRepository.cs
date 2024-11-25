using Microsoft.EntityFrameworkCore;
using friendly.Models;

namespace friendly.DAL
{
    public class LikeRepository : ILikeRepository
    {
        private readonly PostDbContext _db;
        private readonly ILogger<LikeRepository> _logger;

        public LikeRepository(PostDbContext db, ILogger<LikeRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<bool> Create(Like like)
        {
            try
            {
                // Sjekk om en like allerede eksisterer
                var existingLike = await _db.Likes
                    .FirstOrDefaultAsync(l => l.PostId == like.PostId && l.UserId == like.UserId);

                if (existingLike != null)
                {
                    return false; // Returner false hvis like allerede finnes
                }

                // Legg til like hvis den ikke finnes
                await _db.Likes.AddAsync(like);
                await _db.SaveChangesAsync();
                return true; // Returner true hvis like ble lagt til
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while creating like for PostId: {PostId} and UserId: {UserId}", like.PostId, like.UserId);
                return false; // Return false ved feil
            }
        }

        public async Task<bool> DeleteByPostAndUser(int postId, string userId)
        {
            try
            {
                var like = await _db.Likes
                    .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);
                
                if (like != null)
                {
                    _db.Likes.Remove(like);
                    await _db.SaveChangesAsync();
                    return true; // Returner true hvis like ble slettet
                }
                
                return false; // Return false hvis like ikke finnes
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while deleting like for PostId: {PostId} and UserId: {UserId}", postId, userId);
                return false; // Return false ved feil
            }
        }
        
        public async Task<int> GetLikesCount(int postId)
        {
            return await _db.Likes.CountAsync(l => l.PostId == postId);
        }
    }
}
            
    





