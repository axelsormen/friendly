using Microsoft.AspNetCore.Mvc;
using friendly.DAL;
using friendly.Models;
using friendly.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace friendly.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentRepository _commentRepository; // Repository for interacting with comments
        private readonly ILogger<CommentController> _logger; // Logger for logging errors and info
        private readonly UserManager<User> _userManager;  // UserManager to access user data and manage users

        public CommentController(ICommentRepository commentRepository, ILogger<CommentController> logger, UserManager<User> userManager)
        {
            _commentRepository = commentRepository;
            _logger = logger; // Initialize logger
            _userManager = userManager;  // Initialize UserManager to get user details
        }
        
        [HttpGet]
        [Authorize]  // Only authorized users can create a comment
        public IActionResult Create()
        {
            _logger.LogInformation("Create GET action called.");
            return View();
        }

        [HttpPost]
        [Authorize]  // Only authorized users can post comments
        public async Task<IActionResult> Create(Comment comment)
        {
            _logger.LogInformation("Create POST action called with Comment data: {@Comment}", comment);

            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("User not found while creating comment.");
                return Unauthorized();
            }

            // Assign the user's ID to the comment and set the current date as the comment's date
            comment.UserId = user.Id;
            comment.CommentDate = DateTime.Now.ToString();

            // Check if the model state is valid before proceeding
            if (ModelState.IsValid)
            {
                try
                {
                    // Create the comment in the database via the repository
                    await _commentRepository.Create(comment);
                    _logger.LogInformation("Comment created successfully with ID: {CommentId}", comment.CommentId);
                    
                    // Redirect back to the previous page after successful creation
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while creating the comment.");
                    ModelState.AddModelError("", "An error occurred while saving your comment.");
                }
            }
            return Redirect(Request.Headers["Referer"].ToString()); // If ModelState is invalid, redirect back
        }

        [HttpGet]
        [Authorize] // Only authorized users can update a comment
        public async Task<IActionResult> Update(int id)
        {
            try
            {
                var comment = await _commentRepository.GetCommentById(id);
                if (comment == null)
                {
                    _logger.LogError("[CommentController] Comment not found when updating the CommentId {CommentId:0000}", id);
                    return NotFound("Comment not found for the CommentId");
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized("User is not logged in.");
                }

                if (comment.UserId != user.Id)
                {
                    return Forbid(); // Ensure only the comment creator can update the comment
                }

                return View(comment); // Return the comment data for editing
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CommentController] Error occurred while retrieving the comment for update.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost]
        [Authorize] // Only authorized users can update a comment
        public async Task<IActionResult> Update(Comment comment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var originalComment = await _commentRepository.GetCommentById(comment.CommentId);
                    if (originalComment == null)
                    {
                        _logger.LogError("[CommentController] Original comment not found for update, CommentId: {CommentId:0000}", comment.CommentId);
                        return NotFound("Original comment not found.");
                    }

                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        return Unauthorized("User is not logged in.");
                    }

                    if (originalComment.UserId != user.Id)
                    {
                        return Forbid(); // Ensure only the comment creator can update the comment
                    }

                    originalComment.CommentText = comment.CommentText; // Update the comment's text

                    bool returnOk = await _commentRepository.Update(originalComment);
                    if (returnOk)
                        return RedirectToAction("Table", "Post"); // Redirect back to Table if the update is successful
                        _logger.LogError("[CommentController] Comment update failed for CommentId {CommentId:0000}.", comment.CommentId);
                }

                return View(comment); // Return the view if update fails
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CommentController] Error occurred while updating the comment.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet]
        [Authorize]  // Only authorized users can attempt to delete comments
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Delete GET action called for CommentId: {CommentId}", id);
            
            // Retrieve the comment to be deleted from the repository
            var comment = await _commentRepository.GetCommentById(id);
            if (comment == null)
            {
                _logger.LogError("[CommentController] Comment not found for CommentId {CommentId}", id);
                return BadRequest("Comment not found for the CommentId");
            }

            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);  
            if (user == null)
            {
                _logger.LogError("User not found during delete attempt.");
                return Unauthorized(); // Return Unauthorized if user is not found
            }

            // Check if the logged-in user owns the comment (only the owner can delete it)
            if (comment.UserId != user.Id)  
            {
                _logger.LogWarning("User {UserId} attempted to delete comment {CommentId} but does not have ownership.", user.Id, id);
                return Forbid();  
            }

            // Render the confirmation view for deletion
            _logger.LogInformation("Rendering confirmation view for deleting CommentId: {CommentId}", id);
            return View(comment);
        }

        [HttpPost]
        [Authorize]  // Only authorized users can delete comments
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("DeleteConfirmed POST action called for CommentId: {CommentId}", id);

            // Retrieve the comment to be deleted from the repository
            var comment = await _commentRepository.GetCommentById(id);
            if (comment == null)
            {
                _logger.LogError("[CommentController] Comment not found for CommentId {CommentId}", id);
                return BadRequest("Comment not found for the CommentId");
            }

            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);  
            if (user == null)
            {
                _logger.LogError("User not found during delete confirmation.");
                return Unauthorized(); // Return Unauthorized if user is not found
            }

            // Check if the logged-in user owns the comment (only the owner can delete it)
            if (comment.UserId != user.Id)
            {
                _logger.LogWarning("User {UserId} attempted to delete comment {CommentId} but is not the owner.", user.Id, id);
                return Forbid();  
            }

            // Attempt to delete the comment through the repository
            bool returnOk = await _commentRepository.Delete(id);
            if (!returnOk)
            {
                _logger.LogError("[CommentController] Comment deletion failed for CommentId {CommentId}", id);
                return BadRequest("Comment deletion failed");
            }

            _logger.LogInformation("Successfully deleted CommentId: {CommentId}", id);

            // Redirect back to the previous page after successful deletion
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
