using Microsoft.AspNetCore.Mvc;
using friendly.DAL;
using friendly.Models;
using friendly.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
namespace friendly.Controllers;

public class CommentController : Controller
{
    private readonly ICommentRepository _commentRepository;
    private readonly ILogger<CommentController> _logger; // Logger for logging errors and info
    private readonly UserManager<User> _userManager;  // UserManager to access user data

    public CommentController(ICommentRepository commentRepository, ILogger<CommentController> logger, UserManager<User> userManager)
    {
        _commentRepository = commentRepository;
        _logger = logger; // Initialize Logger
        _userManager = userManager;  // Initialize UserManager
    }
    
    [HttpGet]
    [Authorize]
    public IActionResult Create()
    {
        _logger.LogInformation("Create GET action called.");
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(Comment comment)
    {
        _logger.LogInformation("Create POST action called with Comment data: {@Comment}", comment);

        // Ensure the UserId and PostId are set
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            _logger.LogError("User not found while creating comment.");
            return Unauthorized();
        }

        comment.UserId = user.Id;
        comment.CommentDate = DateTime.Now.ToString();

        if (ModelState.IsValid)
        {
            try
            {
                await _commentRepository.Create(comment);
                _logger.LogInformation("Comment created successfully with ID: {CommentId}", comment.CommentId);
                
                // Redirect back to the same page after successful submission
                return Redirect(Request.Headers["Referer"].ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the comment.");
                ModelState.AddModelError("", "An error occurred while saving your comment.");
            }
        } 
        return Redirect(Request.Headers["Referer"].ToString()); // Redirect back to the same page if ModelState is invalid
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Delete GET action called for CommentId: {CommentId}", id);
        
        var comment = await _commentRepository.GetCommentById(id);
        if (comment == null)
        {
            _logger.LogError("[CommentController] Comment not found for CommentId {CommentId}", id);
            return BadRequest("Comment not found for the CommentId");
        }

        // Ownership check
        var user = await _userManager.GetUserAsync(User);  // Get the logged-in user
        if (user == null)
        {
            _logger.LogError("User not found during delete attempt.");
            return Unauthorized();
        }
        if (comment.UserId != user.Id)  
        {
            _logger.LogWarning("User {UserId} attempted to delete comment {CommentId} but does not have ownership.", user.Id, id);
            return Forbid();  
        }

        _logger.LogInformation("Rendering confirmation view for deleting CommentId: {CommentId}", id);
        return View(comment);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        _logger.LogInformation("DeleteConfirmed POST action called for CommentId: {CommentId}", id);

        // Retrieve the comment from the repository
        var comment = await _commentRepository.GetCommentById(id);
        if (comment == null)
        {
            _logger.LogError("[CommentController] Comment not found for CommentId {CommentId}", id);
            return BadRequest("Comment not found for the CommentId");
        }

        // Ownership check
        var user = await _userManager.GetUserAsync(User);  
        if (user == null)
        {
            _logger.LogError("User not found during delete confirmation.");
            return Unauthorized();
        }

        if (comment.UserId != user.Id)
        {
            _logger.LogWarning("User {UserId} attempted to delete comment {CommentId} but is not the owner.", user.Id, id);
            return Forbid();  
        }

        bool returnOk = await _commentRepository.Delete(id);
        if (!returnOk)
        {
            _logger.LogError("[CommentController] Comment deletion failed for CommentId {CommentId}", id);
            return BadRequest("Comment deletion failed");
        }

        _logger.LogInformation("Successfully deleted CommentId: {CommentId}", id);

        // Redirect back to the origin page after successful deletion
        return Redirect(Request.Headers["Referer"].ToString());
    }
}