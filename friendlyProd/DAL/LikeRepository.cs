using Microsoft.EntityFrameworkCore;
using friendly.Models;

namespace friendly.DAL
{
    public class LikeRepository : ILikeRepository
    {
        private readonly PostDbContext _db;
        private readonly ILogger<LikeRepository> _logger;

        // Constructor to inject dependencies for database context and logger
        public LikeRepository(PostDbContext db, ILogger<LikeRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        // Method to create a like 
        public async Task<bool> Create(Like like)
        {
            try
            {
                // Check if a like already exists for this post and user
                var existingLike = await _db.Likes
                    .FirstOrDefaultAsync(l => l.PostId == like.PostId && l.UserId == like.UserId);

                if (existingLike != null)
                {
                    return false;
                }

                // Add the new like if it does not already exist
                await _db.Likes.AddAsync(like);
                await _db.SaveChangesAsync();
                return true; 
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while creating like for PostId: {PostId} and UserId: {UserId}", like.PostId, like.UserId);
                return false;
            }
        }

        // Method to delete a like for a post by a user
        public async Task<bool> DeleteByPostAndUser(int postId, string userId)
        {
            try
            {
                // Find the like based on the post ID and user ID
                var like = await _db.Likes
                    .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);
                
                if (like != null)
                {
                    _db.Likes.Remove(like);  // Remove the like if found
                    await _db.SaveChangesAsync();
                    return true;
                }
                
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while deleting like for PostId: {PostId} and UserId: {UserId}", postId, userId);
                return false;
            }
        }
        
        // Method to get the count of likes for a specific post
        public async Task<int> GetLikesCount(int postId)
        {
            try
            {
                // Return the count of likes for the given post
                return await _db.Likes.CountAsync(l => l.PostId == postId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while counting likes for PostId: {PostId}", postId);
                return 0; // Return 0 in case of an error
            }
        }
    }
}
