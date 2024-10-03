using Microsoft.EntityFrameworkCore;
using friendly.Models;

namespace friendly.Models
{
    public static class DBInit
    {
        public static void Seed(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<PostDbContext>();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Declare users, posts, and comments outside the if-blocks
            List<User> users = new List<User>();
            List<Post> posts = new List<Post>();
            List<Comment> comments = new List<Comment>();

            if (!context.Users.Any())
            {
                users = new List<User>
                {
                    new User { Name = "Axel Ørmen", Username = "axelsormen", Email = "axelsormen@gmail.com", ProfileImageUrl = "https://www.npg.org.uk/assets/image-cache//npg-image-crops/square.e41b7b6f.william_shakespeare.e6eb8891.jpg" },
                    new User { Name = "Kristoffer Kvam", Username = "kvammy", Email = "kvamsy@gmail.com", ProfileImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR34TYEL0DubGu70_6cKyEa4EX55BzWn28Zsg&s" },
                    new User { Name = "Jesper Schwartz", Username = "jezzper", Email = "jeppe@gmail.com", ProfileImageUrl = "pb3" }
                };
                context.Users.AddRange(users);
                context.SaveChanges();
            }
            else
            {
                users = context.Users.ToList();  // Fetch existing users if already present
            }

            if (!context.Posts.Any())
            {
                posts = new List<Post>
                {
                    new Post { PostImagePath = "/uploads/mountains.jpg", Caption = "Koser meg på fjelltur", PostDate = DateTime.Now.ToString(), UserId = users[0].UserId },
                    new Post { PostImagePath = "/uploads/fall.jpg", Caption = "Høsten er nydelig", PostDate = DateTime.Now.ToString(), UserId = users[1].UserId },
                    new Post { PostImagePath = "/uploads/beach.jpg", Caption = "Stranda på denne tiden av døgnet er magisk", PostDate = DateTime.Now.ToString(), UserId = users[0].UserId }
                };
                context.Posts.AddRange(posts);
                context.SaveChanges();
            }
            else
            {
                posts = context.Posts.ToList();  // Fetch existing posts if already present
            }

            if (!context.Comments.Any())
            {
                comments = new List<Comment>
                {
                    new Comment { CommentText = "Ser koselig ut!", CommentDate = DateTime.Now.ToString(), PostId = posts[1].PostId, UserId = users[0].UserId },
                    new Comment { CommentText = "Flott!", CommentDate = DateTime.Now.ToString(), PostId = posts[0].PostId, UserId = users[1].UserId },
                    new Comment { CommentText = "Du er en rå fotograf!", CommentDate = DateTime.Now.ToString(), PostId = posts[1].PostId, UserId = users[1].UserId }
                };
                context.Comments.AddRange(comments);
                context.SaveChanges();
            }
            else
            {
                comments = context.Comments.ToList();  // Fetch existing comments if already present
            }
        }
    }
}
