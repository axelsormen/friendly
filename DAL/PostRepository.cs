using Microsoft.EntityFrameworkCore; // Required for async operations
using friendly.Models;

namespace friendly.DAL
{
    public class PostRepository : IPostRepository
    {
        private readonly PostDbContext _context;

        public PostRepository(PostDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Post>> GetAll()
        {
            return await _context.Posts.ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPostsByUserId(int userId)
        {
            return await _context.Posts.Where(p => p.UserId == userId).ToListAsync();
        }

        public async Task<Post?> GetPostById(int id)
        {
            return await _context.Posts.FindAsync(id);
        }

        public async Task Create(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Post post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Delete(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return false;

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
