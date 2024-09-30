using Microsoft.AspNetCore.Mvc;
using friendly.DAL;
using friendly.Models;
using friendly.ViewModels;
using Microsoft.Extensions.Logging; // Add this for ILogger

namespace friendly.Controllers;

public class PostController : Controller
{
    private readonly IPostRepository _postRepository;
    private readonly ILogger<PostController> _logger;

    public PostController(IPostRepository postRepository, ILogger<PostController> logger)
    {
        _postRepository = postRepository;
        _logger = logger;
    }

public async Task<IActionResult> Table()
{
    var posts = await _postRepository.GetAll();
    var postsViewModel = new PostsViewModel(posts, "Table");
    return View(postsViewModel);
}

public async Task<IActionResult> Grid()
{
    var posts = await _postRepository.GetAll();
    var postsViewModel = new PostsViewModel(posts, "Grid");
    return View(postsViewModel); // Make sure you're passing PostsViewModel
}
    
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

[HttpPost]
public async Task<IActionResult> Create(Post post, IFormFile imageFile)
{
    _logger.LogInformation("Create action called with Post data: {@Post}", post);

    // Handle the image file
    if (imageFile != null && imageFile.Length > 0)
    {
        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

        if (!Directory.Exists(uploadsDir))
        {
            Directory.CreateDirectory(uploadsDir);
        }

        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
        var filePath = Path.Combine(uploadsDir, uniqueFileName);

        // Save the file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        // Assign the file path to PostImagePath
        post.PostImagePath = $"/uploads/{uniqueFileName}";
    }
    else
    {
        ModelState.AddModelError("PostImagePath", "An image file is required.");
    }

    // Set the post date
    post.PostDate = DateTime.Now.ToString();  

    // Validate the model after all fields are properly set
    if (ModelState.IsValid)
    {
        try
        {
            await _postRepository.Create(post);
            _logger.LogInformation("Post created successfully with ID: {PostId}", post.PostId);
            return RedirectToAction(nameof(Table));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the post.");
        }
    }

    // Log validation errors
    foreach (var state in ModelState)
    {
        foreach (var error in state.Value.Errors)
        {
            _logger.LogWarning("Validation error on field {Field}: {ErrorMessage}", state.Key, error.ErrorMessage);
        }
    }

    return View(post);  // Return the view with validation errors
}

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var post = await _postRepository.GetPostById(id);
        if (post == null)
        {
            return NotFound();
        }
        return View(post);
    }

[HttpPost]
public async Task<IActionResult> Update(Post post)
{
    // Log that the Update action has been called
    _logger.LogInformation("Update action called with Post data: {@Post}", post);

    if (ModelState.IsValid)
    {
        try
        {
            // Retrieve the original post from the database
            var originalPost = await _postRepository.GetPostById(post.PostId);
            if (originalPost == null)
            {
                return NotFound();  // Return not found if the post doesn't exist
            }

            // Preserve unchanged properties
            originalPost.Caption = post.Caption; // Only update the Caption

            // Log before updating the post
            _logger.LogInformation("Model is valid, updating post...");

            // Update the post in the repository
            await _postRepository.Update(originalPost);  // Update with the original post object

            // Log successful post update
            _logger.LogInformation("Post updated successfully with ID: {PostId}", post.PostId);

            return RedirectToAction(nameof(Table));
        }
        catch (Exception ex)
        {
            // Log any exception that occurs
            _logger.LogError(ex, "An error occurred while updating the post.");
            return View(post);
        }
    }

    // Log validation errors if ModelState is invalid
    _logger.LogWarning("ModelState is invalid. Errors: {@Errors}", ModelState.Values.SelectMany(v => v.Errors));
    return View(post);
}

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _postRepository.GetPostById(id);
        if (post == null)
        {
            return NotFound();
        }
        return View(post);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _postRepository.Delete(id);
        return RedirectToAction(nameof(Table));
    }
}