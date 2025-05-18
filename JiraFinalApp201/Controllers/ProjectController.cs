using JiraFinalApp201.Models.Database;
using JiraFinalApp201.Models.Projects;
using JiraFinalApp201.Services.Projects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JiraFinalApp201.Controllers
{
    public class ProjectController : Controller
    {
        private readonly JiraFinalApp201Db _context;

        public ProjectController(JiraFinalApp201Db context)
        {
            _context = context;
        }

        // GET: Project/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // POST: Project/Create
        [HttpPost]
        
        public IActionResult Create(Project project)
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                project.CreatedDate = DateTime.Now;
                _context.Add(project);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }
        // GET: Project/Index
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            var projects = _context.Projects.ToList();
            return View(projects);
        }
    }
}
