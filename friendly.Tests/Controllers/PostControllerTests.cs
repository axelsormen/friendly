using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using friendly.Controllers;
using friendly.DAL;
using friendly.Models;
using friendly.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Http; 

namespace friendly.Tests.Controllers
{
    public class PostControllerTests
    {
        private readonly Mock<IPostRepository> _mockPostRepository;
        private readonly Mock<ILogger<PostController>> _mockLogger;

        public PostControllerTests()
        {
            _mockPostRepository = new Mock<IPostRepository>();
            _mockLogger = new Mock<ILogger<PostController>>();
        }

        // Positive test: Verify posts are returned from the Table view
        [Fact]
        public async Task Table_ReturnsViewWithPosts()
        {
            // Arrange
            var postList = new List<Post>
            {
                new Post { PostId = 1, Caption = "Test 1", PostDate = "2024-01-01", UserId = "User1" },
                new Post { PostId = 2, Caption = "Test 2", PostDate = "2024-01-02", UserId = "User2" }
            };
            _mockPostRepository.Setup(repo => repo.GetAll()).ReturnsAsync(postList);
            var controller = new PostController(_mockPostRepository.Object, _mockLogger.Object, null);

            // Act
            var result = await controller.Table();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<PostsViewModel>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Posts.Count());
        }

        // Negative test: When there are no posts, the view should still return with an empty list or message.
        [Fact]
        public async Task Table_ReturnsViewWithEmptyListWhenNoPosts()
        {
            // Arrange
            _mockPostRepository.Setup(repo => repo.GetAll()).ReturnsAsync(new List<Post>());
            var controller = new PostController(_mockPostRepository.Object, _mockLogger.Object, null);

            // Act
            var result = await controller.Table();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<PostsViewModel>(viewResult.ViewData.Model);
            Assert.Empty(model.Posts);
        }

        // Positive test: Check if new post is created and redirected
        [Fact]
        public async Task Create_Post_ValidData_RedirectsToTable()
        {
            // Arrange
            var post = new Post { PostId = 1, Caption = "New Post", PostDate = "2024-01-01", UserId = "User1" };
            _mockPostRepository.Setup(repo => repo.Create(post)).ReturnsAsync(true);

            // Mock UserManager
            var mockUserStore = new Mock<IUserStore<User>>();
            var mockUserManager = new Mock<UserManager<User>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
            var user = new User { Id = "User1", UserName = "TestUser" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var controller = new PostController(_mockPostRepository.Object, _mockLogger.Object, mockUserManager.Object);

            // Simulate file upload
            var mockImageFile = new Mock<IFormFile>();
            mockImageFile.Setup(f => f.Length).Returns(1024L); 
            mockImageFile.Setup(f => f.FileName).Returns("test.jpg");

            // Act
            var result = await controller.Create(post, mockImageFile.Object);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Table", redirectResult.ActionName); 
        }

        // Negative test: Invalid post creation, should return view with errors
        [Fact]
        public async Task Create_Post_InvalidData_ReturnsViewWithModelErrors()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
            var user = new User { Id = "User1", UserName = "TestUser" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var controller = new PostController(_mockPostRepository.Object, _mockLogger.Object, mockUserManager.Object);

            var post = new Post { PostId = 1, Caption = "", PostDate = "2024-01-01", UserId = "User1" }; // Invalid data (empty caption)
            controller.ModelState.AddModelError("Caption", "Required");

            // Act
            var result = await controller.Create(post, null);  // Pass null for the image file

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(post, viewResult.Model);
        }

        // Positive test: Valid post update and redirection
        [Fact]
        public async Task Update_ValidPost_RedirectsToTable()
        {
            // Arrange
            var post = new Post { PostId = 1, Caption = "Updated Post", PostDate = "2024-01-01", UserId = "User1" };
            _mockPostRepository.Setup(repo => repo.GetPostById(post.PostId)).ReturnsAsync(post);
            _mockPostRepository.Setup(repo => repo.Update(post)).ReturnsAsync(true);
            var controller = new PostController(_mockPostRepository.Object, _mockLogger.Object, null);

            // Act
            var result = await controller.Update(post);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Table", redirectResult.ActionName);
        }

        // Negative test: Invalid update (empty caption), should return view with validation errors
        [Fact]
        public async Task Update_InvalidPost_ReturnsViewWithModelError()
        {
            // Arrange
            var post = new Post { PostId = 1, Caption = "", PostDate = "2024-01-01", UserId = "User1" }; // Invalid data
            var controller = new PostController(_mockPostRepository.Object, _mockLogger.Object, null);
            controller.ModelState.AddModelError("Caption", "Required");

            // Act
            var result = await controller.Update(post);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(post, viewResult.Model);
        }

        // Positive test: Delete an existing post
        [Fact]
        public async Task Delete_PostExists_ReturnsView()
        {
            // Arrange
            var post = new Post { PostId = 1, Caption = "Post to Delete", PostDate = "2024-01-01", UserId = "User1" };
            _mockPostRepository.Setup(repo => repo.GetPostById(post.PostId)).ReturnsAsync(post);
            var controller = new PostController(_mockPostRepository.Object, _mockLogger.Object, null);

            // Act
            var result = await controller.Delete(post.PostId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(post, viewResult.Model);
        }

        // Negative test: Try deleting a non-existing post (error handling)
        [Fact]
        public async Task Delete_PostNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockPostRepository.Setup(repo => repo.GetPostById(It.IsAny<int>())).ReturnsAsync((Post)null);
            var controller = new PostController(_mockPostRepository.Object, _mockLogger.Object, null);

            // Act
            var result = await controller.Delete(999); // Non-existing post

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        // Positive test: Successfully delete a post
        [Fact]
        public async Task DeleteConfirmed_ValidPost_RedirectsToTable()
        {
            // Arrange
            var post = new Post { PostId = 1, Caption = "Post to Delete", PostDate = "2024-01-01", UserId = "User1" };
            var postId = 1;

            // Mock GetPostById to return the post
            _mockPostRepository.Setup(repo => repo.GetPostById(postId)).ReturnsAsync(post);
            
            // Mock Delete to return true (indicating the post was successfully deleted)
            _mockPostRepository.Setup(repo => repo.Delete(postId)).ReturnsAsync(true);
            
            var controller = new PostController(_mockPostRepository.Object, _mockLogger.Object, null);

            // Act
            var result = await controller.DeleteConfirmed(postId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Table", redirectResult.ActionName);
        }

        // Negative test: Try deleting a post that doesn't exist
        [Fact]
        public async Task DeleteConfirmed_PostNotFound_ReturnsNotFound()
        {
            // Arrange
            var postId = 999;
            _mockPostRepository.Setup(repo => repo.Delete(postId)).ReturnsAsync(false);
            var controller = new PostController(_mockPostRepository.Object, _mockLogger.Object, null);

            // Act
            var result = await controller.DeleteConfirmed(postId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}