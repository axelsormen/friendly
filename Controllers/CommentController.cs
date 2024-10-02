using Microsoft.AspNetCore.Mvc;
using friendly.DAL;
using friendly.Models;
using friendly.ViewModels;
using Microsoft.Extensions.Logging; // Add this for ILogger

namespace friendly.Controllers;

public class CommentController : Controller
{
    private readonly ICommentRepository _commentRepository;
    private readonly ILogger<CommentController> _logger;

    public CommentController(ICommentRepository commentRepository, ILogger<CommentController> logger)
    {
        _commentRepository = commentRepository;
        _logger = logger;
    }
    
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Comment comment)
    {
        _logger.LogInformation("Create action called with Comment data: {@Comment}", comment);

        comment.CommentDate = DateTime.Now.ToString();

        if (ModelState.IsValid)
        {
            try
            {
                await _commentRepository.Create(comment);
                _logger.LogInformation("Comment created successfully with ID: {CommentId}", comment.CommentId);
                
                // Return to the same page after submission
                return Redirect(Request.Headers["Referer"].ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the comment.");
            }
        }

        foreach (var state in ModelState)
        {
            foreach (var error in state.Value.Errors)
            {
                _logger.LogWarning("Validation error on field {Field}: {ErrorMessage}", state.Key, error.ErrorMessage);
            }
        }

        return View(comment);
    }


    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var comment = await _commentRepository.GetCommentById(id);
        if (comment == null)
        {
            _logger.LogError("[CommentController] Comment not found when updating the CommentId {CommentId:0000}", id);
            return BadRequest("Comment not found for the CommentId");
        }
        return View(comment);
    }

    [HttpPost]
    public async Task<IActionResult> Update(Comment comment)
    {
        if (ModelState.IsValid)
        {
            var originalComment = await _commentRepository.GetCommentById(comment.CommentId);
            originalComment.CommentText = comment.CommentText;

            bool returnOk = await _commentRepository.Update(originalComment);
            if(returnOk)
                return Redirect(Request.Headers["Referer"].ToString());
            }
        _logger.LogWarning("[CommentController] Comment update failed {@comment}", comment);
        return View(comment);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var comment = await _commentRepository.GetCommentById(id);
        if (comment == null)
        {
            _logger.LogError("[CommentController] Comment not found for the CommentId {CommentId:0000}", id);
            return BadRequest("Comment not found for the CommentId");
        }
        return View(comment);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        bool returnOk = await _commentRepository.Delete(id);
        if(!returnOk)
        {
            _logger.LogError("[CommentController] Comment deletion failed for the CommentId {CommentId:0000}", id);
            return BadRequest("Comment deletion failed");
        }
        return Redirect(Request.Headers["Referer"].ToString());
    }
}