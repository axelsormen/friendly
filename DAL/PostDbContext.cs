using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using friendly.Models;

namespace friendly.Models
{
    public class PostDbContext : IdentityDbContext<User> // Use your custom User class here
    {
        public PostDbContext(DbContextOptions<PostDbContext> options) : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define the relationship between Post and User
            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)  // Each Post has one User
                .WithMany(u => u.Posts) // A User can have many Posts
                .HasForeignKey(p => p.UserId); // Foreign key is UserId in Post

            // Define the relationship between Comment and Post
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post) // Each Comment is associated with one Post
                .WithMany(p => p.Comments) // A Post can have many Comments
                .HasForeignKey(c => c.PostId); // Foreign key is PostId in Comment
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
