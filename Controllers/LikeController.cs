using Microsoft.AspNetCore.Mvc;
using friendly.DAL;
using friendly.Models;
using friendly.ViewModels;
using Microsoft.Extensions.Logging;
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
        public async Task<IActionResult> Create(Like like)
        {
            _logger.LogInformation("Create action called with Like data: {@Like}", like);

            if (ModelState.IsValid)
            {
                try
                {
                    await _likeRepository.Create(like);
                    _logger.LogInformation("Like created successfully with ID: {LikeId}", like.LikeId);
                    
                    // Return to the same page after submission
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while creating the like.");
                }
            }

            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    _logger.LogWarning("Validation error on field {Field}: {ErrorMessage}", state.Key, error.ErrorMessage);
                }
            }

            return View(like);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool returnOk = await _likeRepository.Delete(id);
            if(!returnOk)
            {
                _logger.LogError("[LikeController] Like deletion failed for the LikeId {LikeId:0000}", id);
                return BadRequest("Like deletion failed");
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}