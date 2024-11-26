using Microsoft.AspNetCore.Mvc;
using friendly.DAL;
using friendly.Models;
using friendly.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace friendly.Controllers;

public class PostController : Controller
{
    private readonly IPostRepository _postRepository;
    private readonly ILogger<PostController> _logger; // Add Logger to log errors
    private readonly UserManager<User> _userManager; // Add UserManager to access user data

    public PostController(IPostRepository postRepository, ILogger<PostController> logger, UserManager<User> userManager)
    {
        _postRepository = postRepository;
        _logger = logger; // Initialize Logger
        _userManager = userManager;  // Initialize UserManager
    }

    public async Task<IActionResult> Table()
    {
        var posts = await _postRepository.GetAll();
        if (posts == null)
        {
            _logger.LogError("[PostController] Post list not found while executing _postRepository.GetAll()");
            return NotFound("Post list not found");
        }
        var postsViewModel = new PostsViewModel(posts, "Table");
        return View(postsViewModel);
    }

    public async Task<IActionResult> Grid()
    {
        var posts = await _postRepository.GetAll();
        if (posts == null)
        {
            _logger.LogError("[PostController] Post list not found while executing _postRepository.GetAll()");
            return NotFound("Post list not found");
        }
        var postsViewModel = new PostsViewModel(posts, "Grid");
        return View(postsViewModel);
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

        // Get the current logged-in user
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            _logger.LogError("[PostController] Unable to retrieve the logged-in user.");
            return Unauthorized("You need to be logged in to create a post.");
        }

        // Set the UserId to the currently logged-in user's ID
        post.UserId = user.Id;
        _logger.LogInformation("UserId being assigned to post: {UserId}", post.UserId);

        // Check if an image file has been provided
        if (imageFile != null && imageFile.Length > 0)
        {
            _logger.LogInformation("[PostController] Image file received: {FileName}", imageFile.FileName);

            // Validate the file extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                _logger.LogWarning("[PostController] Invalid file extension: {FileExtension}", fileExtension);
                ModelState.AddModelError("PostImagePath", "Only .jpg, .jpeg, and .png image formats are allowed.");
                return View(post);  // Return View if validation fails
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
                _logger.LogError("[PostController] Error while saving image: {ErrorMessage}", ex.Message);
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

        // Only proceed if ModelState is valid
        if (ModelState.IsValid)
        {
            _logger.LogInformation("[PostController] ModelState is valid. Attempting to create the post.");
            bool returnOk = await _postRepository.Create(post);
            if (returnOk)
            {
                _logger.LogInformation("[PostController] Post created successfully. Redirecting to Table.");
                return RedirectToAction(nameof(Table));  // Ensure this redirect happens
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

        _logger.LogWarning("[PostController] Post creation failed {@post}", post);
        return View(post);  // Ensure this is returned only when ModelState is invalid or creation fails
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Update(int id)
    {
        var post = await _postRepository.GetPostById(id);
        if (post == null)
        {
            _logger.LogError("[PostController] Post not found when updating the PostId {PostId:0000}", id);
            return BadRequest("Post not found for the PostId");
        }

        // Ownership check
        var user = await _userManager.GetUserAsync(User); // Get the logged-in user
        if (user == null)
        {
            return Unauthorized("User is not logged in.");
        }

        if (post.UserId != user.Id)
        {                
            return Forbid();  // 403 Forbidden if user is not the owner
        }
        return View(post);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Update(Post post)
    {
        if (ModelState.IsValid)
        {
            var originalPost = await _postRepository.GetPostById(post.PostId);
            // Ownership check
            var user = await _userManager.GetUserAsync(User);

            if (originalPost == null)
            {
                return NotFound("Original post not found.");
            }

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
        }

        _logger.LogWarning("[PostController] Post update failed {@post}", post);
        return View(post);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _postRepository.GetPostById(id);
        if (post == null)
        {
            _logger.LogError("[PostController] Post not found for the PostId {PostId:0000}", id);
            return NotFound("Post not found for the PostId");  // Return 404 if post is not found
        }
        // Ownership check:
        var user = await _userManager.GetUserAsync(User); // Get the logged-in user
        if (user == null)
        {
            return Unauthorized("User is not logged in.");
        }

        if (post == null)
        {
            return NotFound("Post not found.");
        }

        if (post.UserId != user.Id)
        {
            return Forbid();
        }
        return View(post);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var post = await _postRepository.GetPostById(id);
        if (post == null)
        {
            return NotFound();
        }

        // Ownership check
        var user = await _userManager.GetUserAsync(User); // Get the logged-in user
        if (user == null)
        {
            return Unauthorized("User is not logged in.");
        }

        if (post == null)
        {
            return NotFound("Post not found.");
        }

        if (post.UserId != user.Id)
        {
            return Forbid();
        }

        var result = await _postRepository.Delete(id);
        if (result)
        {
            // Redirect back to the origin page
            return RedirectToAction("Table");
        }
        return NotFound();
    }
}