using Microsoft.AspNetCore.Mvc;
using friendly.DAL;
using friendly.Models;
using friendly.ViewModels;
using Microsoft.Extensions.Logging; // Add this for ILogger

namespace friendly.Controllers;

public class UserController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserRepository userRepository, IPostRepository postRepository, ILogger<UserController> logger)
    {
        _userRepository = userRepository;
        _postRepository = postRepository;
        _logger = logger;
    }

    public async Task<IActionResult> Details(int id)
    {

        var user = await _userRepository.GetUserById(id);
        if (user == null)
        {
            _logger.LogError("[UserController] User not found for the UserId {UserId:0000}", id);
            return NotFound();
        }

        var posts = await _postRepository.GetPostsByUserId(id);
        if (posts == null)
        {
            _logger.LogError("[UserController] Posts not found for the UserId {UserId:0000}", id);
            return NotFound();
        }

        var viewModel = new UsersViewModel(user, posts, "User Details");
        return View(viewModel);
    }
}
