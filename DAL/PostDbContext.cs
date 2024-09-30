using Microsoft.EntityFrameworkCore;
using friendly.Models;

namespace friendly.Models;

public class PostDbContext : DbContext
{
	public PostDbContext(DbContextOptions<PostDbContext> options) : base(options)
	{
        // Database.EnsureCreated();  // Remove this line if you use migrations
	}

	public DbSet<Post> Posts { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Thread> Threads { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }
}