using Microsoft.EntityFrameworkCore; // Required for async operations
using friendly.Models;
using Microsoft.AspNetCore.Identity; // Include this for IdentityUser
using Microsoft.Extensions.Logging; // Include this for ILogger

namespace friendly.DAL
{
    public class PostRepository : IPostRepository
    {
        private readonly PostDbContext _db; // The database context to interact with the database
        private readonly ILogger<PostRepository> _logger; // Logger for logging errors or information

        public PostRepository(PostDbContext db, ILogger<PostRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        // Retrieves all posts ordered by the date in descending order
        public async Task<IEnumerable<Post>?> GetAll()
        {
            try
            {
                // Order posts by date (newest first)
                return await _db.Posts.OrderByDescending(p => p.PostDate).ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("[PostRepository] posts ToListAsync() failed when GetAll(), error message: {e}", e.Message);
                return null;
            }
        }

        // Retrieves all posts created by a specific user identified by userId
        public async Task<IEnumerable<Post>?> GetPostsByUserId(string userId) // Change to string for IdentityUser
        {
            try
            {
                // Fetch posts by the given userId
                return await _db.Posts.Where(p => p.UserId == userId).ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("[PostRepository] Failed to get posts for UserId {UserId}, error message: {ErrorMessage}", userId, e.Message);
                return null;
            }
        }

        // Retrieves a post by its unique ID
        public async Task<Post?> GetPostById(int id)
        {
            try
            {
                // Find post by its ID
                return await _db.Posts.FindAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogError("[PostRepository] post FindAsync(id) failed when GetPostById for PostId {PostId:0000}, error message: {e}", id, e.Message);
                return null;
            }
        }

        // Adds a new post to the database
        public async Task<bool> Create(Post post)
        {
            try
            {
                _logger.LogInformation("[PostRepository] Adding post to the DbContext: {@post}", post);
                
                // Add the new post to the DbSet
                _db.Posts.Add(post);

                _logger.LogInformation("[PostRepository] Entity state before saving: {State}", _db.Entry(post).State);

                // Save changes to the database
                await _db.SaveChangesAsync();

                _logger.LogInformation("[PostRepository] Entity state after saving: {State}", _db.Entry(post).State);
                _logger.LogInformation("[PostRepository] Post creation succeeded with PostId: {PostId}", post.PostId);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[PostRepository] post creation failed for post {@post}, error message: {e}", post, e.Message);
                return false; 
            }        
        }

        // Updates an existing post in the database
        public async Task<bool> Update(Post post)
        {
            try
            {
                // Mark the post as updated in the DbSet
                _db.Posts.Update(post);

                // Save changes to the database
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[PostRepository] post FindAsync(id) failed when updating the PostId {PostId:0000}, error message {e}", post.PostId, e.Message);
                return false;
            }
        }

        // Deletes a post by its ID
        public async Task<bool> Delete(int id)
        {
            try
            {
                // Find the post by its ID
                var post = await _db.Posts.FindAsync(id);

                // If post not found, log error and return false
                if (post == null)
                {
                    _logger.LogError("[PostRepository] post not found for the PostId {PostId:0000}", id);
                    return false;
                }

                // Remove the post from the DbSet
                _db.Posts.Remove(post);

                // Save changes to the database
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[PostRepository] post deletion failed for the PostId {PostId:0000}, error message: {e}", id, e.Message);
                return false;
            }
        }
    }
}