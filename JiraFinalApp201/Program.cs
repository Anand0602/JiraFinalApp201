using JiraFinalApp201.Middleware;
using JiraFinalApp201.Services.Tasks;
using JiraFinalApp201.Services.Projects;
using JiraFinalApp201.Services.User;
using JiraFinalApp201.Models.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC setup
builder.Services.AddControllersWithViews();

// DB context setup with SQL Server
builder.Services.AddDbContext<JiraFinalApp201Db>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper for object mapping
builder.Services.AddAutoMapper(typeof(Program));

// DI for service layer
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IUserService, UserService>();

// Session config - 30 min timeout with secure cookies
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Error handling for production
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

// Request logging for diagnostics
app.UseMiddleware<RequestLoggingMiddleware>();

// Enable session state
app.UseSession();

app.UseRouting();

app.UseAuthorization();

// Default route points to login page
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
