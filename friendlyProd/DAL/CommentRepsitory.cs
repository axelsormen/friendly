using Microsoft.EntityFrameworkCore; // Required for async operations
using friendly.Models;

namespace friendly.DAL
{
    public class CommentRepository : ICommentRepository
    {
        private readonly PostDbContext _db;
        private readonly ILogger<CommentRepository> _logger;

     public CommentRepository(PostDbContext db, ILogger<CommentRepository> logger)
        {
            _db = db;
            _logger = logger;
        }
          public async Task<IEnumerable<Comment>?> GetAll()
        {
            try
            {
                return await _db.Comments.ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("[CommentRepository] comments ToListAsync() failed when GetAll(), error message: {e}", e.Message);
                return null;
            }
        }

    public async Task<Comment?> GetCommentById(int id)
    {
        try{
            return await _db.Comments.FindAsync(id);
        }
        catch (Exception e)
        {
            _logger.LogError("[CommentRepository] comment FindAsync(id) failed when GetCommentById for CommentId {CommentId:0000}, error message: {e}", id, e.Message);
            return null;
        }
    }

         public async Task<bool> Create(Comment comment)
        {
            try{
                _db.Comments.Add(comment);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[CommentRepository] comment creation failed for comment {@comment}, error message: {e}", comment, e.Message);
                return false;
            }        
        }

        public async Task<bool> Update(Comment comment)
        {
            try{
                _db.Comments.Update(comment);
                await _db.SaveChangesAsync();
                return true;
            }
            catch(Exception e)
            {
                _logger.LogError("[CommentRepository] comment FindAsync(id) failed when updating the CommentId {CommentId:0000}, error message {e}", comment, e.Message);
                return false;
            }
        }

 public async Task<bool> Delete(int id)
        {
            try
            {
                var comment = await _db.Comments.FindAsync(id);
                if(comment == null)
                {
                    _logger.LogError("[CommentRepository] comment not found for the CommentId {CommentId:0000}", id);
                    return false;
                }

                _db.Comments.Remove(comment);
                await _db.SaveChangesAsync();
                return true;
            }
            catch(Exception e)
            {
                _logger.LogError("[CommentRepository] comment deletion failed for the CommentId {CommentId:0000}, error message: {e}", id, e.Message);
                return false;
            }
        }
    }
}
