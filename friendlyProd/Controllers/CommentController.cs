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
    private readonly ILogger<CommentController> _logger; // Add Logger to log errors
    private readonly UserManager<User> _userManager;  // Add UserManager to access user data

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
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(Comment comment)
    {
        _logger.LogInformation("Create action called with Comment data: {@Comment}", comment);

        // Ensure the UserId and PostId are set
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            _logger.LogError("User not found.");
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
        return Redirect(Request.Headers["Referer"].ToString()); // Redirect back to the same page
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var comment = await _commentRepository.GetCommentById(id);
        if (comment == null)
        {
            _logger.LogError("[CommentController] Comment not found for the CommentId {CommentId:0000}", id);
            return BadRequest("Comment not found for the CommentId");
        }

        //Ownership check
        var user = await _userManager.GetUserAsync(User);  // Get the logged-in user
        if (user == null)
        {
            _logger.LogError("User not found.");
            return Unauthorized();
        }
        if (comment.UserId != user.Id)  
        {
            return Forbid();  
        }

        // Redirect Comment view for confirmation
        return View(comment);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {

        // Retrieve the comment from the repository
        var comment = await _commentRepository.GetCommentById(id);
        if (comment == null)
        {
            _logger.LogError("[CommentController] Comment not found for the CommentId {CommentId:0000}", id);
            return BadRequest("Comment not found for the CommentId");
        }

        // Ownership check
        var user = await _userManager.GetUserAsync(User);  
        if (user == null)
        {
            _logger.LogError("User not found.");
            return Unauthorized();
        }

        if (comment.UserId != user.Id)
        {
            return Forbid();  
        }

        bool returnOk = await _commentRepository.Delete(id);
        if(!returnOk)
        {
            _logger.LogError("[CommentController] Comment deletion failed for the CommentId {CommentId:0000}", id);
            return BadRequest("Comment deletion failed");
        }

        // Redirect back to the origin page
        return Redirect(Request.Headers["Referer"].ToString());
    }
}