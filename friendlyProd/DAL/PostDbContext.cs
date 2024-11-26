using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using friendly.Models;

namespace friendly.Models
{
    public class PostDbContext : IdentityDbContext<User>
    {
        // Constructor that passes options to the base IdentityDbContext
        public PostDbContext(DbContextOptions<PostDbContext> options) : base(options)
        {
        }

        // DbSets for the entities that will be stored in the database
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Calls the base class method to apply the default Identity model configuration

            // Define the relationship between Post and User
            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)  // Each Post is associated with one User
                .WithMany(u => u.Posts) // A User can have many Posts
                .HasForeignKey(p => p.UserId); // UserId is the foreign key in the Post entity

            // Define the relationship between Comment and Post
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post) // Each Comment is associated with one Post
                .WithMany(p => p.Comments) // A Post can have many Comments
                .HasForeignKey(c => c.PostId); // PostId is the foreign key in the Comment entity
                
            // Define the relationship between Like and Post
            modelBuilder.Entity<Like>()
                .HasOne(l => l.Post) // Each Like is associated with one Post
                .WithMany(p => p.Likes) // A Post can have many Likes
                .HasForeignKey(l => l.PostId); // PostId is the foreign key in the Like entity

            // Create a unique constraint on the combination of PostId and UserId to prevent duplicate likes
            modelBuilder.Entity<Like>()
                .HasIndex(l => new { l.PostId, l.UserId }) // Composite index on PostId and UserId
                .IsUnique(); // Ensures a user can only like a post once
        }

        // This method is used to configure additional options for the DbContext
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies(); // Enable lazy loading of related entities
        }
    }
}
