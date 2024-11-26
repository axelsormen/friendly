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
        private readonly UserManager<User> _userManager; // UserManager for managing users
        private readonly IPostRepository _postRepository; // Repository to interact with posts
        private readonly ILogger<UserController> _logger; // Logger for logging information and errors

        public UserController(UserManager<User> userManager, IPostRepository postRepository, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _postRepository = postRepository;
            _logger = logger;
        }

        // Action to display all users in a table
        public async Task<IActionResult> Table()
        {
            try
            {
                // Fetch all users asynchronously from the database
                var users = await _userManager.Users.ToListAsync();
                
                if (users == null || !users.Any()) // Check if no users are found
                {
                    _logger.LogError("[UserController] No users found in the database.");
                    return NotFound("User list not found.");
                }

                // Create a ViewModel that holds the list of users for the table view
                var usersViewModel = new UsersViewModel(users, "Table");

                return View(usersViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[UserController] Error occurred while fetching users for Table view.");
                return StatusCode(500, "Internal server error.");
            }
        }

        // Action to display details of a specific user
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) // Check if the user ID is invalid or missing
                {
                    _logger.LogError("[UserController] Invalid or missing user ID.");
                    return BadRequest("User ID cannot be null or empty."); // Return a BadRequest if the ID is invalid
                }

                // Find the user by ID using UserManager
                var user = await _userManager.FindByIdAsync(id);
                if (user == null) // If user not found
                {
                    _logger.LogError("[UserController] User not found for the UserId {UserId}", id);
                    return NotFound($"User with ID {id} not found.");
                }

                // Fetch the posts associated with this user
                var posts = await _postRepository.GetPostsByUserId(id);
                if (posts == null || !posts.Any()) // If no posts found for the user
                {
                    _logger.LogWarning("[UserController] No posts found for the UserId {UserId}. Returning empty list.", id);
                    posts = new List<Post>();
                }

                // Create a ViewModel to pass user details and their posts to the view
                var viewModel = new UsersViewModel(user, posts, "User Details");

                return View(viewModel); // Return the View with the ViewModel
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[UserController] Error occurred while fetching user details for UserId {UserId}.", id);
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}