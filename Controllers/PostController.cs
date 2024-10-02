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
    if(posts == null)
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
    if(posts == null)
    {
        _logger.LogError("[PostController] Post list not found while executing _postRepository.GetAll()");
        return NotFound("Post list not found");
    }
    var postsViewModel = new PostsViewModel(posts, "Grid");
    return View(postsViewModel); 
}
    
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Post post, IFormFile imageFile)
    {

        if (imageFile != null && imageFile.Length > 0)
        {
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
            var filePath = Path.Combine(uploadsDir, uniqueFileName);

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

        if (ModelState.IsValid)
        {
            bool returnOk = await _postRepository.Create(post);
            if(returnOk)
                return RedirectToAction(nameof(Table));
        }
        _logger.LogWarning("[PostController] Post creation failed {@post}", post);
        return View(post);
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var post = await _postRepository.GetPostById(id);
        if (post == null)
        {
            _logger.LogError("[PostController] Post not found when updating the PostId {PostId:0000}", id);
            return BadRequest("Post not found for the PostId");
        }
        return View(post);
    }

    [HttpPost]
    public async Task<IActionResult> Update(Post post)
    {
        if (ModelState.IsValid)
        {
            var originalPost = await _postRepository.GetPostById(post.PostId);
            originalPost.Caption = post.Caption;

            bool returnOk = await _postRepository.Update(originalPost);
            if(returnOk)
                return RedirectToAction(nameof(Table));
            }
        _logger.LogWarning("[PostController] Post update failed {@post}", post);
        return View(post);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _postRepository.GetPostById(id);
        if (post == null)
        {
            _logger.LogError("[PostController] Post not found for the PostId {PostId:0000}", id);
            return BadRequest("Post not found for the PostId");
        }
        return View(post);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        bool returnOk = await _postRepository.Delete(id);
        if(!returnOk)
        {
            _logger.LogError("[PostController] Post deletion failed for the PostId {PostId:0000}", id);
            return BadRequest("Post deletion failed");
        }
        return RedirectToAction(nameof(Table));
    }
}