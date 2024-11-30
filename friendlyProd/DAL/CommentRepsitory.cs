using Microsoft.EntityFrameworkCore;
using friendly.Models;

namespace friendly.DAL
{
    // CommentRepository implements ICommentRepository
    public class CommentRepository : ICommentRepository
    {
        private readonly PostDbContext _db; // The database context for interacting with the database
        private readonly ILogger<CommentRepository> _logger; // Logger to log errors

        // Constructor to initialize the database context and logger
        public CommentRepository(PostDbContext db, ILogger<CommentRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        // Retrieves all comments from the database
        public async Task<IEnumerable<Comment>?> GetAll()
        {
            try
            {
                // Asynchronously fetches all comments from the Comments table
                return await _db.Comments.ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("[CommentRepository] comments ToListAsync() failed when GetAll(), error message: {e}", e.Message);
                return null;
            }
        }

        // Retrieves a specific comment by its ID
        public async Task<Comment?> GetCommentById(int id)
        {
            try
            {
                // Asynchronously finds a comment by its ID in the Comments table
                return await _db.Comments.FindAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogError("[CommentRepository] comment FindAsync(id) failed when GetCommentById for CommentId {CommentId:0000}, error message: {e}", id, e.Message);
                return null;
            }
        }

        // Creates a new comment in the database
        public async Task<bool> Create(Comment comment)
        {
            try
            {
                // Adds the new comment to the Comments table
                _db.Comments.Add(comment);

                // Asynchronously saves the changes to the database
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[CommentRepository] comment creation failed for comment {@comment}, error message: {e}", comment, e.Message);
                return false;
            }
        }

        // Updates a comment by its ID
        public async Task<bool> Update(Comment comment)
        {
            try
            {
                // Asynchronously finds the comment by its ID
                var existingComment = await _db.Comments.FindAsync(comment.CommentId);

                if (existingComment == null)
                {
                    _logger.LogError("[CommentRepository] comment not found for the CommentId {CommentId:0000}", comment.CommentId);
                    return false;
                }

                // Update the comment's properties with the new data
                existingComment.CommentText = comment.CommentText; // Update other properties if needed

                // Asynchronously save changes to the database
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[CommentRepository] comment update failed for the CommentId {CommentId:0000}, error message: {e}", comment.CommentId, e.Message);
                return false;
            }
        }

        // Deletes a comment by its ID
        public async Task<bool> Delete(int id)
        {
            try
            {
                // Asynchronously finds the comment by its ID
                var comment = await _db.Comments.FindAsync(id);

                // Checks if the comment exists
                if (comment == null)
                {
                    _logger.LogError("[CommentRepository] comment not found for the CommentId {CommentId:0000}", id);
                    return false;
                }

                // Removes the comment from the Comments table
                _db.Comments.Remove(comment);

                // Asynchronously saves the changes to the database
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[CommentRepository] comment deletion failed for the CommentId {CommentId:0000}, error message: {e}", id, e.Message);
                return false;
            }
        }
    }
}