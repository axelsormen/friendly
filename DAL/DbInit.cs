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
                        PhoneNumber = "+47 12345678",
                        EmailConfirmed = false,
                    },
                    new User
                    {
                        FirstName = "Kristoffer",
                        LastName = "Kvam",
                        UserName = "kvammy",
                        Email = "kvamsy@gmail.com",
                        ProfileImageUrl = "/uploads/profile-images/profileimage2.jpg",
                        PhoneNumber = "+47 18811881",
                        EmailConfirmed = false,
                    },
                    new User
                    {
                        FirstName = "Simen",
                        LastName = "Thams",
                        UserName = "sthams",
                        Email = "simenthams@hotmail.com",
                        ProfileImageUrl = "/uploads/profile-images/profileimage3.jpg",
                        EmailConfirmed = false,
                    },
                    new User
                    {
                        FirstName = "Adina",
                        LastName = "Heia",
                        UserName = "adinah",
                        Email = "aheia@hotmail.com",
                        ProfileImageUrl = "/uploads/profile-images/profileimage4.jpg",
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
            var simen = await userManager.FindByEmailAsync("simenthams@hotmail.com");
            var adina = await userManager.FindByEmailAsync("aheia@hotmail.com");

            // Seed posts
            if (!context.Posts.Any())
            {
                var posts = new List<Post>
                {
                    new Post { PostImagePath = "/uploads/mountains.jpg", Caption = "Enjoying the mountains", PostDate = DateTime.Now.ToString(), UserId = axel.Id },
                    new Post { PostImagePath = "/uploads/fall.jpg", Caption = "Autumn is beautiful", PostDate = DateTime.Now.ToString(), UserId = kristoffer.Id },
                    new Post { PostImagePath = "/uploads/beach.jpg", Caption = "Loving the beach!", PostDate = DateTime.Now.ToString(), UserId = axel.Id }
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
                    new Comment { CommentText = "Awesome!", CommentDate = DateTime.Now.ToString(), PostId = allPosts[1].PostId, UserId = axel.Id },
                    new Comment { CommentText = "Beautiful, wow!", CommentDate = DateTime.Now.ToString(), PostId = allPosts[0].PostId, UserId = kristoffer.Id },
                    new Comment { CommentText = "You are such a great photographer", CommentDate = DateTime.Now.ToString(), PostId = allPosts[1].PostId, UserId = adina.Id }
                };

                context.Comments.AddRange(comments);
                await context.SaveChangesAsync();
            }
        }
    }
}
