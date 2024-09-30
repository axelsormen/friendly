using Microsoft.EntityFrameworkCore; // Required for async operations
using friendly.Models;

namespace friendly.DAL
{
    public class CommentRepository : ICommentRepository
    {
        private readonly PostDbContext _context;

        public CommentRepository(PostDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Comment>> GetAll()
        {
            return await _context.Comments.ToListAsync();
        }

        public async Task<Comment?> GetCommentById(int id)
        {
            return await _context.Comments.FindAsync(id);
        }

        public async Task Create(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Delete(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return false;

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
