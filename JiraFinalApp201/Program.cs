using JiraFinalApp201.Middleware;
using JiraFinalApp201.Services.Tasks;
using JiraFinalApp201.Services.Projects;
using JiraFinalApp201.Services.User;
using JiraFinalApp201.Models.Database;  // Make sure this is the namespace for JiraFinalApp201Db
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register DbContext with SQL Server connection string (make sure it's in appsettings.json)
builder.Services.AddDbContext<JiraFinalApp201Db>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register AutoMapper if you use it
builder.Services.AddAutoMapper(typeof(Program));

// Register application services with scoped lifetime
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IUserService, UserService>();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

// Custom middleware
app.UseMiddleware<RequestLoggingMiddleware>();

// Use session
app.UseSession();

app.UseRouting();

app.UseAuthorization();

// Setup default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
