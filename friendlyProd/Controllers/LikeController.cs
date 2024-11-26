using Microsoft.AspNetCore.Mvc;
using friendly.DAL;
using friendly.Models;
using Microsoft.AspNetCore.Authorization;

namespace friendly.Controllers
{
    public class LikeController : Controller
    {
        private readonly ILikeRepository _likeRepository; // Repository to interact with the Like data
        private readonly ILogger<LikeController> _logger; // Logger for logging errors and information

        public LikeController(ILikeRepository likeRepository, ILogger<LikeController> logger)
        {
            _likeRepository = likeRepository;
            _logger = logger;
        }

        [HttpPost]
        [Authorize] // Only authorized users can create a like
        public async Task<IActionResult> Create(int postId, string userId)
        {
            var like = new Like
            {
                PostId = postId, // Assign the PostId for the like
                UserId = userId  // Assign the UserId for the like
            };

            _logger.LogInformation("Create action called with Like data: {@Like}", like);

            try
            {
                // Try to create the like using the repository
                bool result = await _likeRepository.Create(like);

                if (!result)
                {
                    _logger.LogWarning("Like already exists for postId {PostId} and userId {UserId}", postId, userId);
                    return Conflict("Like already exists");
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
        [Authorize] // Only authorized users can delete a like
        public async Task<IActionResult> DeleteConfirmed(int postId, string userId)
        {
            // Try to delete the like from the repository
            bool returnOk = await _likeRepository.DeleteByPostAndUser(postId, userId);

            if (!returnOk)
            {
                _logger.LogError("[LikeController] Like deletion failed for postId {PostId} and userId {UserId}", postId, userId);
                return BadRequest("Like deletion failed");
            }

            _logger.LogInformation("[LikeController] Like deleted successfully for postId {PostId} and userId {UserId}", postId, userId);
            return Ok();
        }

        [HttpGet]
        [Authorize] // Only authorized users can view the like count
        public async Task<IActionResult> GetLikesCount(int postId)
        {
            try
            {
                // Retrieve the like count from the repository
                var count = await _likeRepository.GetLikesCount(postId);
                _logger.LogInformation("Like count retrieved successfully for PostId: {PostId}, Count: {Count}", postId, count);
                
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the like count for PostId: {PostId}", postId);
                return BadRequest("Unable to retrieve like count");
            }
        }
    }
}