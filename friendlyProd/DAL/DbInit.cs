using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using friendly.Models;

namespace friendly.Models
{
    public static class DBInit
    {
        // Seed method that will populate the database with initial data if it's empty
        public static async Task Seed(IApplicationBuilder app)
        {
            // Creates a service scope to get required services from DI container
            using var serviceScope = app.ApplicationServices.CreateScope();
            
            // Getting the necessary services
            var context = serviceScope.ServiceProvider.GetRequiredService<PostDbContext>(); // Database context
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>(); // User manager for managing users
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(); // Role manager for managing roles

            // Ensures the database is deleted and recreated
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Seed roles (if they don't exist already)
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Seed users if there are no users in the database
            if (!context.Users.Any())
            {
                // Create a list of users to be added
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
                        EmailConfirmed = false, // Email not confirmed by default
                    },
                    new User
                    {
                        FirstName = "Kristoffer",
                        LastName = "Kvam",
                        UserName = "kvammy",
                        Email = "kvamsy@gmail.com",
                        ProfileImageUrl = "/uploads/profile-images/profileimage2.jpg",
                        PhoneNumber = "+47 18811881",
                        EmailConfirmed = false, // Email not confirmed by default
                    },
                    new User
                    {
                        FirstName = "Simen",
                        LastName = "Thams",
                        UserName = "sthams",
                        Email = "simenthams@hotmail.com",
                        ProfileImageUrl = "/uploads/profile-images/profileimage3.jpg",
                        EmailConfirmed = false, // Email not confirmed by default
                    },
                    new User
                    {
                        FirstName = "Adina",
                        LastName = "Heia",
                        UserName = "adinah",
                        Email = "aheia@hotmail.com",
                        ProfileImageUrl = "/uploads/profile-images/profileimage4.jpg",
                        EmailConfirmed = false, // Email not confirmed by default
                    }
                };

                // Loop through the users and add them to the database with a default password
                foreach (var user in users)
                {
                    var result = await userManager.CreateAsync(user, "P@ssw0rd!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "User");
                    }
                }
            }

            // Retrieve the users by email to ensure they exist in the database
            var axel = await userManager.FindByEmailAsync("axelsormen@gmail.com");
            if (axel == null)
            {
                throw new InvalidOperationException("User 'axelsormen@gmail.com' not found during seeding.");
            }

            var kristoffer = await userManager.FindByEmailAsync("kvamsy@gmail.com");
            if (kristoffer == null)
            {
                throw new InvalidOperationException("User 'kvamsy@gmail.com' not found during seeding.");
            }

            var simen = await userManager.FindByEmailAsync("simenthams@hotmail.com");
            if (simen == null)
            {
                throw new InvalidOperationException("User 'simenthams@hotmail.com' not found during seeding.");
            }

            var adina = await userManager.FindByEmailAsync("aheia@hotmail.com");
            if (adina == null)
            {
                throw new InvalidOperationException("User 'aheia@hotmail.com' not found during seeding.");
            }

            // Seed posts if there are no posts in the database
            if (!context.Posts.Any())
            {
                // Create a list of posts to be added
                var posts = new List<Post>
                {
                    new Post { 
                        PostImagePath = "/uploads/mountains.jpg", 
                        Caption = "Enjoying the mountains", 
                        PostDate = DateTime.Now.ToString(), 
                        UserId = axel.Id }, // Assign the post to Axel
                    new Post { 
                        PostImagePath = "/uploads/fall.jpg", 
                        Caption = "Autumn is beautiful", 
                        PostDate = DateTime.Now.ToString(),
                        UserId = kristoffer.Id }, // Assign the post to Kristoffer
                    new Post { 
                        PostImagePath = "/uploads/beach.jpg", 
                        Caption = "Loving the beach!", 
                        PostDate = DateTime.Now.ToString(),
                        UserId = axel.Id } // Assign the post to Axel
                };

                // Add the posts to the database
                context.Posts.AddRange(posts);
                await context.SaveChangesAsync();
            }

            // Seed comments if there are no comments in the database
            var allPosts = context.Posts.ToList();

            if (!context.Comments.Any())
            {
                // Create a list of comments to be added
                var comments = new List<Comment>
                {
                    new Comment { 
                        CommentText = "Awesome!", 
                        CommentDate = DateTime.Now.ToString(), 
                        PostId = allPosts[1].PostId, // Associate comment with the second post
                        UserId = axel.Id }, // Axel is the user who commented
                    new Comment { 
                        CommentText = "Beautiful, wow!", 
                        CommentDate = DateTime.Now.ToString(), 
                        PostId = allPosts[0].PostId, // Associate comment with the first post
                        UserId = kristoffer.Id }, // Kristoffer is the user who commented
                    new Comment { 
                        CommentText = "You are such a great photographer", 
                        CommentDate = DateTime.Now.ToString(), 
                        PostId = allPosts[1].PostId, // Associate comment with the second post
                        UserId = adina.Id } // Adina is the user who commented
                };

                // Add the comments to the database
                context.Comments.AddRange(comments);
                await context.SaveChangesAsync();
            }
        }
    }
}