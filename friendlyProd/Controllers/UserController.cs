using Microsoft.AspNetCore.Mvc;
using friendly.DAL;
using friendly.Models;
using friendly.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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
            try
            {
                // Fetch all users asynchronously
                var users = await _userManager.Users.ToListAsync();
                
                if (users == null || !users.Any())
                {
                    _logger.LogError("[UserController] No users found in the database.");
                    return NotFound("User list not found.");
                }

                // Pass the list of users to the view model
                var usersViewModel = new UsersViewModel(users, "Table");

                return View(usersViewModel);
            }
            catch (Exception ex)
            {
                // Log any error that occurs while fetching users
                _logger.LogError(ex, "[UserController] Error occurred while fetching users for Table view.");
                return StatusCode(500, "Internal server error.");
            }
        }

        // GET: User/Details/{id}
        public async Task<IActionResult> Details(string id)
        {
            try
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
                    _logger.LogWarning("[UserController] No posts found for the UserId {UserId}. Returning empty list.", id);
                    posts = new List<Post>(); // Initialize to an empty list if no posts found
                }

                // Create a view model with user and posts
                var viewModel = new UsersViewModel(user, posts, "User Details");

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log any error that occurs while fetching user details
                _logger.LogError(ex, "[UserController] Error occurred while fetching user details for UserId {UserId}.", id);
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}