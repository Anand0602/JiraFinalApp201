using JiraFinalApp201.Models.Database;
using JiraFinalApp201.Models.Authentication;
using JiraFinalApp201.Models.Database;
using JiraFinalApp201.Models.Tasks;

using JiraFinalApp201.Services.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JiraFinalApp201.Controllers
{
    public class AccountController : Controller
    {
        private readonly JiraFinalApp201Db _context;
        private readonly IUserService _userService; 

        public AccountController(JiraFinalApp201Db context, IUserService userService )
        {
            _context = context;
            _userService = userService;
            
        }

        // GET:
        public IActionResult Login()
        {
            try
            {
                 
                if (HttpContext.Session.GetString("UserId") != null)
                {
                    return RedirectToAction("Index", "Board");
                }

                return View();
            }
            catch (Exception ex)
            {
                 
                TempData["ErrorMessage"] = "Database connection error. Please check your connection string and ensure SQL Server is running.";
                return RedirectToAction("Error", "Home");
            }
        }
        [HttpPost]

        public IActionResult Login(string username, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    ModelState.AddModelError("", "Username and password are required");
                    return View();
                }

                var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

                if (user != null)
                {
                    // Store user info in session
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("Username", user.Username);
                   

                    return RedirectToAction("Index", "Board");
                }

                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }
            catch (Exception ex)
            {
               
                ModelState.AddModelError("", "An error occurred during login. Please check your database connection.");
                return View();
            }
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            try
            {
                return View(new User());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Database connection error. Please check your connection string and ensure SQL Server is running.";
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Account/Register
        [HttpPost]
        public IActionResult Register(User user)
        {
            try
            {
                // Server-side validation for required fields
                if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
                {
                    ModelState.AddModelError("", "Email and password are required.");
                    return View(user);
                }

                // Validate the model attributes (like [EmailAddress], [Required], etc.)
                if (!ModelState.IsValid)
                {
                    return View(user);
                }

                // Check if email already exists
                bool emailExists;
                try
                {
                    emailExists = _context.Users.Any(u => u.Email == user.Email);
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Database connection error. Please try again later.";
                    return RedirectToAction("Error", "Home");
                }

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "An account with this email already exists.");
                    return View(user);
                }

                // Save the new user
                _context.Users.Add(user);
                _context.SaveChanges();

                // Auto-login after registration
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("Email", user.Email);

                return RedirectToAction("Index", "Board");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred during registration. Please try again.";
                return RedirectToAction("Error", "Home");
            }
        }



        // GET: Account/Logout
        public IActionResult Logout()
        {
            try
            {
                // Clear session
                HttpContext.Session.Clear();

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
             
                return RedirectToAction("Login");
            }
        }

    }
}
