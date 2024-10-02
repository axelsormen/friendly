using Microsoft.EntityFrameworkCore; // Required for async operations
using friendly.Models;

namespace friendly.DAL
{
    public class PostRepository : IPostRepository
    {
        private readonly PostDbContext _db;
        private readonly ILogger<PostRepository> _logger;

        public PostRepository(PostDbContext db, ILogger<PostRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Post>?> GetAll()
        {
            try
            {
                return await _db.Posts.ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("[PostRepository] posts ToListAsync() failed when GetAll(), error message: {e}", e.Message);
                return null;
            }
        }

public async Task<IEnumerable<Post>?> GetPostsByUserId(int userId)
{
    try
    {
        return await _db.Posts.Where(p => p.UserId == userId).ToListAsync();
    }
    catch (Exception e)
    {
        _logger.LogError("[PostRepository] Failed to get posts for UserId {UserId:0000}, error message: {ErrorMessage}", userId, e.Message);
        return null;
    }
}


    public async Task<Post?> GetPostById(int id)
    {
        try{
            return await _db.Posts.FindAsync(id);
        }
        catch (Exception e)
        {
            _logger.LogError("[PostRepository] post FindAsync(id) failed when GetPostById for PostId {PostId:0000}, error message: {e}", id, e.Message);
            return null;
        }
    }

        public async Task<bool> Create(Post post)
        {
            try{
                _db.Posts.Add(post);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[PostRepository] post creation failed for post {@post}, error message: {e}", post, e.Message);
                return false;
            }        
        }

        public async Task<bool> Update(Post post)
        {
            try{
                _db.Posts.Update(post);
                await _db.SaveChangesAsync();
                return true;
            }
            catch(Exception e)
            {
                _logger.LogError("[PostRepository] post FindAsync(id) failed when updating the PostId {PostId:0000}, error message {e}", post, e.Message);
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var post = await _db.Posts.FindAsync(id);
                if(post == null)
                {
                    _logger.LogError("[PostRepository] post not found for the PostId {PostId:0000}", id);
                    return false;
                }

                _db.Posts.Remove(post);
                await _db.SaveChangesAsync();
                return true;
            }
            catch(Exception e)
            {
                _logger.LogError("[PostRepository] post deletion failed for the PostId {PostId:0000}, error message: {e}", id, e.Message);
                return false;
            }
        }
    }
}
