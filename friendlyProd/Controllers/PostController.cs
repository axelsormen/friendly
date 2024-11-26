using Microsoft.AspNetCore.Mvc;
using friendly.DAL;
using friendly.Models;
using friendly.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System.Linq;

namespace friendly.Controllers;

public class PostController : Controller
{
    private readonly IPostRepository _postRepository;
    private readonly ILogger<PostController> _logger; // Logger for logging errors and info
    private readonly UserManager<User> _userManager; // UserManager to access user data

    public PostController(IPostRepository postRepository, ILogger<PostController> logger, UserManager<User> userManager)
    {
        _postRepository = postRepository;
        _logger = logger; // Initialize Logger
        _userManager = userManager;  // Initialize UserManager
    }

    public async Task<IActionResult> Table()
    {
        try
        {
            var posts = await _postRepository.GetAll();
            if (posts == null || !posts.Any())
            {
                _logger.LogError("[PostController] Post list not found or is empty while executing _postRepository.GetAll()");
                return NotFound("Post list not found.");
            }
            var postsViewModel = new PostsViewModel(posts, "Table");
            return View(postsViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[PostController] Error occurred while retrieving posts in Table action.");
            return StatusCode(500, "Internal server error.");
        }
    }

    public async Task<IActionResult> Grid()
    {
        try
        {
            var posts = await _postRepository.GetAll();
            if (posts == null || !posts.Any())
            {
                _logger.LogError("[PostController] Post list not found or is empty while executing _postRepository.GetAll()");
                return NotFound("Post list not found.");
            }
            var postsViewModel = new PostsViewModel(posts, "Grid");
            return View(postsViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[PostController] Error occurred while retrieving posts in Grid action.");
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpGet]
    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(Post post, IFormFile imageFile)
    {
        _logger.LogInformation("[PostController] Create method started.");

        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("[PostController] Unable to retrieve the logged-in user.");
                return Unauthorized("You need to be logged in to create a post.");
            }

            post.UserId = user.Id;
            _logger.LogInformation("UserId assigned to post: {UserId}", post.UserId);

            if (imageFile != null && imageFile.Length > 0)
            {
                _logger.LogInformation("[PostController] Image file received: {FileName}", imageFile.FileName);

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    _logger.LogWarning("[PostController] Invalid file extension: {FileExtension}", fileExtension);
                    ModelState.AddModelError("PostImagePath", "Only .jpg, .jpeg, and .png image formats are allowed.");
                    return View(post);
                }

                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsDir))
                {
                    _logger.LogInformation("[PostController] Uploads directory does not exist, creating directory.");
                    Directory.CreateDirectory(uploadsDir);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                var filePath = Path.Combine(uploadsDir, uniqueFileName);

                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        _logger.LogInformation("[PostController] Saving file to {FilePath}", filePath);
                        await imageFile.CopyToAsync(stream);
                    }

                    post.PostImagePath = $"/uploads/{uniqueFileName}";
                    _logger.LogInformation("[PostController] Image uploaded successfully, PostImagePath set to: {PostImagePath}", post.PostImagePath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[PostController] Error while saving image.");
                    ModelState.AddModelError("PostImagePath", "An error occurred while saving the image.");
                }
            }
            else
            {
                _logger.LogWarning("[PostController] No image file received.");
                ModelState.AddModelError("PostImagePath", "An image file is required.");
            }

            post.PostDate = DateTime.Now.ToString();
            _logger.LogInformation("[PostController] Post date set to: {PostDate}", post.PostDate);

            if (ModelState.IsValid)
            {
                _logger.LogInformation("[PostController] ModelState is valid. Attempting to create the post.");
                bool returnOk = await _postRepository.Create(post);
                if (returnOk)
                {
                    _logger.LogInformation("[PostController] Post created successfully. Redirecting to Table.");
                    return RedirectToAction(nameof(Table));
                }
                else
                {
                    _logger.LogError("[PostController] Post creation failed in repository.");
                    ModelState.AddModelError("", "Post creation failed.");
                }
            }
            else
            {
                _logger.LogWarning("[PostController] ModelState is invalid.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("[PostController] Model error: {ErrorMessage}", error.ErrorMessage);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[PostController] Error occurred while creating post.");
            ModelState.AddModelError("", "An error occurred while creating the post.");
        }

        _logger.LogWarning("[PostController] Post creation failed {@post}", post);
        return View(post);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Update(int id)
    {
        try
        {
            var post = await _postRepository.GetPostById(id);
            if (post == null)
            {
                _logger.LogError("[PostController] Post not found when updating the PostId {PostId:0000}", id);
                return NotFound("Post not found for the PostId");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User is not logged in.");
            }

            if (post.UserId != user.Id)
            {
                return Forbid();
            }

            return View(post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[PostController] Error occurred while retrieving the post for update.");
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Update(Post post)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var originalPost = await _postRepository.GetPostById(post.PostId);
                if (originalPost == null)
                {
                    _logger.LogError("[PostController] Original post not found for update, PostId: {PostId:0000}", post.PostId);
                    return NotFound("Original post not found.");
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized("User is not logged in.");
                }

                if (originalPost.UserId != user.Id)
                {
                    return Forbid();
                }

                originalPost.Caption = post.Caption;

                bool returnOk = await _postRepository.Update(originalPost);
                if (returnOk)
                    return RedirectToAction(nameof(Table));

                _logger.LogError("[PostController] Post update failed for PostId {PostId:0000}.", post.PostId);
            }

            return View(post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[PostController] Error occurred while updating the post.");
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var post = await _postRepository.GetPostById(id);
            if (post == null)
            {
                _logger.LogError("[PostController] Post not found for the PostId {PostId:0000}", id);
                return NotFound("Post not found.");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User is not logged in.");
            }

            if (post.UserId != user.Id)
            {
                return Forbid();
            }

            return View(post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[PostController] Error occurred while retrieving the post for deletion.");
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var post = await _postRepository.GetPostById(id);
            if (post == null)
            {
                _logger.LogError("[PostController] Post not found for deletion, PostId: {PostId:0000}", id);
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User is not logged in.");
            }

            if (post.UserId != user.Id)
            {
                return Forbid();
            }

            var result = await _postRepository.Delete(id);
            if (result)
            {
                _logger.LogInformation("[PostController] Post with PostId {PostId:0000} deleted successfully.", id);
                return RedirectToAction("Table");
            }

            _logger.LogError("[PostController] Failed to delete post with PostId {PostId:0000}.", id);
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[PostController] Error occurred while deleting post with PostId {PostId:0000}.", id);
            return StatusCode(500, "Internal server error.");
        }
    }
}