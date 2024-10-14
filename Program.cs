using Microsoft.EntityFrameworkCore;
using friendly.Models;
using friendly.DAL;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PostDbContextConnection") ?? throw new InvalidOperationException("Connection string 'PostDbContextConnection' not found.");

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<PostDbContext>(options => {
    options.UseSqlite(
        builder.Configuration["ConnectionStrings:PostDbContextConnection"]);
});

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
     // Password settings
     options.Password.RequireDigit = true;
     options.Password.RequiredLength = 8;
     options.Password.RequireNonAlphanumeric = true;
     options.Password.RequireUppercase = true;
     options.Password.RequireLowercase = true;
     options.Password.RequiredUniqueChars = 6;

     // Lockout settings
     options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(60);
     options.Lockout.MaxFailedAccessAttempts = 5;
     options.Lockout.AllowedForNewUsers = true;

     // User settings
     options.User.RequireUniqueEmail = true;

     // Sign-in settings
    // options.SignIn.RequireConfirmedAccount = false; // Set to true if you want email confirmation
 })
 .AddEntityFrameworkStores<PostDbContext>()
 .AddDefaultTokenProviders();

 builder.Services.ConfigureApplicationCookie(options =>
 {
     options.LoginPath = "/Identity/Account/Login"; // Ensure this path is valid
 });

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();

builder.Services.AddRazorPages();
builder.Services.AddSession(options =>
{
     options.Cookie.Name = ".AdventureWorks.Session";
     options.IdleTimeout = TimeSpan.FromSeconds(1800); // 30 minutes
     options.Cookie.IsEssential = true;
 });
 
var loggerConfiguration = new LoggerConfiguration().MinimumLevel.Information().WriteTo.File($"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log");

var logger = loggerConfiguration.CreateLogger();
builder.Logging.AddSerilog(logger);

loggerConfiguration.Filter.ByExcluding(e => e.Properties.TryGetValue("SourceContext", out var value) && e.Level == LogEventLevel.Information && e.MessageTemplate.Text.Contains("Executed DbCommand"));

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    await DBInit.Seed(app);
}

app.UseStaticFiles();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();
app.MapRazorPages();
app.Run();