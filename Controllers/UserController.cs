using Microsoft.AspNetCore.Mvc;
using friendly.DAL;
using friendly.Models;
using friendly.ViewModels;
using Microsoft.Extensions.Logging; // Add this for ILogger

namespace friendly.Controllers;

public class UserController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;  // Ensure you have a repository to get posts
    private readonly ILogger<UserController> _logger;

    public UserController(IUserRepository userRepository, IPostRepository postRepository, ILogger<UserController> logger)
    {
        _userRepository = userRepository;
        _postRepository = postRepository;  // Inject the Post repository
        _logger = logger;
    }

    public async Task<IActionResult> Details(int id)
    {
        // Log the action
        _logger.LogInformation("Fetching details for user with ID: {UserId}", id);
        
        // Fetch user details
        var user = await _userRepository.GetUserById(id);
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", id);
            return NotFound(); // More appropriate than BadRequest
        }

        // Fetch posts for the user
        var posts = await _postRepository.GetPostsByUserId(id);

        // Create the view model
        var viewModel = new UsersViewModel(user, posts, "User Details");

        // Return the view with the view model
        return View(viewModel);
    }
}
