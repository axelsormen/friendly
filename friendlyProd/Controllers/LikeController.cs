using Microsoft.AspNetCore.Mvc;
using friendly.DAL;
using friendly.Models;
using Microsoft.AspNetCore.Authorization;

namespace friendly.Controllers
{
    public class LikeController : Controller
    {
        private readonly ILikeRepository _likeRepository;
        private readonly ILogger<LikeController> _logger;

        public LikeController(ILikeRepository likeRepository, ILogger<LikeController> logger)
        {
            _likeRepository = likeRepository;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(int postId, string userId)
        {
            var like = new Like
            {
                PostId = postId,
                UserId = userId
            };

            _logger.LogInformation("Create action called with Like data: {@Like}", like);

            try
            {
                bool result = await _likeRepository.Create(like);

                if (!result)
                {
                    _logger.LogWarning("Like already exists for postId {PostId} and userId {UserId}", postId, userId);
                    return Conflict("Like already exists"); // Return a 409 Conflict status code
                }

                _logger.LogInformation("Like created successfully with ID: {LikeId}", like.LikeId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the like.");
                return BadRequest("Unable to process Like data");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int postId, string userId)
        {
            bool returnOk = await _likeRepository.DeleteByPostAndUser(postId, userId);
            if (!returnOk)
            
            {
                _logger.LogError("[LikeController] Like deletion failed for postId {PostId} and userId {UserId}", postId, userId);
                return BadRequest("Like deletion failed");
                
            }
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetLikesCount(int postId)
        {
            var count = await _likeRepository.GetLikesCount(postId);
            return Ok(count);
        }
    }
}



