using Microsoft.AspNetCore.Mvc;
using friendly.DAL;
using friendly.Models;
using friendly.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace friendly.Controllers
{
    public class UserController : Controller
{
    private readonly UserManager<User> _userManager; 
    private readonly IPostRepository _postRepository;
    private readonly ILogger<UserController> _logger;

    public UserController(UserManager<User> userManager, IPostRepository postRepository, ILogger<UserController> logger)
    {
        _userManager = userManager;
        _postRepository = postRepository;
        _logger = logger;
    }

    // GET: User/Table
    public async Task<IActionResult> Table()
    {
        // Fetch all users asynchronously
        var users = await _userManager.Users.ToListAsync();

        if (users == null || !users.Any())
        {
            _logger.LogError("[UserController] No users found.");
            return NotFound("User list not found.");
        }

        // Pass the list of users to the view model
        var usersViewModel = new UsersViewModel(users, "Table");

        return View(usersViewModel);
    }

    // GET: User/Details/{id}
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            _logger.LogError("[UserController] Invalid or missing user ID.");
            return BadRequest("User ID cannot be null or empty.");
        }

        // Find user by ID
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            _logger.LogError("[UserController] User not found for the UserId {UserId}", id);
            return NotFound($"User with ID {id} not found.");
        }

        // Fetch posts for the user
        var posts = await _postRepository.GetPostsByUserId(id);
        if (posts == null || !posts.Any())
        {
            _logger.LogError("[UserController] No posts found for the UserId {UserId}", id);
            posts = new List<Post>(); // Initialize to an empty list if no posts found
        }

        // Create a view model with user and posts
        var viewModel = new UsersViewModel(user, posts, "User Details");

        return View(viewModel);
    }
}
}