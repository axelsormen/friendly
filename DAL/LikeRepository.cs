using Microsoft.EntityFrameworkCore;
using friendly.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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

        public async Task<IEnumerable<Like>?> GetAll()
        {
            try
            {
                return await _db.Likes.ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("[LikeRepository] Likes ToListAsync() failed when GetAll(), error message: {e}", e.Message);
                return null;
            }
        }

        public async Task<Like?> GetLikeById(int id)
        {
            try
            {
                return await _db.Likes.FindAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogError("[LikeRepository] Like FindAsync(id) failed when GetLikeById for LikeId {LikeId:0000}, error message: {e}", id, e.Message);
                return null;
            }
        }

        public async Task<bool> Create(Like like)
        {
            try
            {
                _db.Likes.Add(like);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[LikeRepository] Like creation failed for Like {@Like}, error message: {e}", like, e.Message);
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var like = await _db.Likes.FindAsync(id);
                if (like == null)
                {
                    _logger.LogError("[LikeRepository] Like not found for the LikeId {LikeId:0000}", id);
                    return false;
                }

                _db.Likes.Remove(like);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[LikeRepository] Like deletion failed for the LikeId {LikeId:0000}, error message: {e}", id, e.Message);
                return false;
            }
        }
    }
}