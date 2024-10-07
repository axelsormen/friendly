using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using friendly.Models;

namespace friendly.Models
{
    public static class DBInit
    {
        public static async Task Seed(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<PostDbContext>();
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Seed roles (if necessary)
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Seed users
            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                    new User
                    {
                        FirstName = "Axel",
                        LastName = "Ørmen",
                        UserName = "axelsormen",
                        Email = "axelsormen@gmail.com",
                        ProfileImageUrl = "/uploads/profile-images/profileimage1.jpg",
                        EmailConfirmed = false,
                    },
                    new User
                    {
                        FirstName = "Kristoffer",
                        LastName = "Kvam",
                        UserName = "kvammy",
                        Email = "kvamsy@gmail.com",
                        ProfileImageUrl = "/uploads/profile-images/profileimage2.jpg",
                        PhoneNumber = "18811881",
                        EmailConfirmed = false,
                    },
                    new User
                    {
                        FirstName = "Jesper",
                        LastName = "Schwartz",
                        UserName = "jezzper",
                        Email = "jeppe@hotmail.com",
                        ProfileImageUrl = "/uploads/profile-images/profileimage3.jpg",
                        EmailConfirmed = false,
                    }
                };

                foreach (var user in users)
                {
                    var result = await userManager.CreateAsync(user, "P@ssw0rd!");
                    if (result.Succeeded)
                    {
                        // Assign "User" role to each user
                        await userManager.AddToRoleAsync(user, "User");
                    }
                }
            }

            // Get users from the database
            var axel = await userManager.FindByEmailAsync("axelsormen@gmail.com");
            var kristoffer = await userManager.FindByEmailAsync("kvamsy@gmail.com");

            // Seed posts
            if (!context.Posts.Any())
            {
                var posts = new List<Post>
                {
                    new Post { PostImagePath = "/uploads/mountains.jpg", Caption = "Koser meg på fjelltur", PostDate = DateTime.Now.ToString(), UserId = axel.Id },
                    new Post { PostImagePath = "/uploads/fall.jpg", Caption = "Høsten er nydelig", PostDate = DateTime.Now.ToString(), UserId = kristoffer.Id },
                    new Post { PostImagePath = "/uploads/beach.jpg", Caption = "Stranda på denne tiden av døgnet er magisk", PostDate = DateTime.Now.ToString(), UserId = axel.Id }
                };

                context.Posts.AddRange(posts);
                await context.SaveChangesAsync();
            }

            // Seed comments
            var allPosts = context.Posts.ToList();  // Get all posts

            if (!context.Comments.Any())
            {
                var comments = new List<Comment>
                {
                    new Comment { CommentText = "Ser koselig ut!", CommentDate = DateTime.Now.ToString(), PostId = allPosts[1].PostId, UserId = axel.Id },
                    new Comment { CommentText = "Flott!", CommentDate = DateTime.Now.ToString(), PostId = allPosts[0].PostId, UserId = kristoffer.Id },
                    new Comment { CommentText = "Du er en rå fotograf!", CommentDate = DateTime.Now.ToString(), PostId = allPosts[1].PostId, UserId = kristoffer.Id }
                };

                context.Comments.AddRange(comments);
                await context.SaveChangesAsync();
            }
        }
    }
}
