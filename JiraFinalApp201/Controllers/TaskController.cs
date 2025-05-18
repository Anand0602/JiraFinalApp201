using Microsoft.AspNetCore.Mvc;
using JiraFinalApp201.Models.Tasks;
using static JiraFinalApp201.Models.Tasks.Taskitems;
using JiraFinalApp201.Models.Database;
using JiraFinalApp201.Services.Tasks;
using JiraFinalApp201.Services.Projects;
using JiraFinalApp201.Services.User;

namespace JiraFinalApp201.Controllers
{
    
   public class TaskController : Controller
    {
        private readonly JiraFinalApp201Db _context;
        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;

        public TaskController(
            JiraFinalApp201Db context,
            ITaskService taskService,
            IProjectService projectService,
            IUserService userService)
        {
            _context = context;
            _taskService = taskService;
            _projectService = projectService;
            _userService = userService;
        }

        // GET: Task/Create
        public async Task<IActionResult> Create(int projectId)
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            var project = await _projectService.GetProjectByIdAsync(projectId);
            if (project == null)
                return NotFound();

            var task = new TaskItem
            {
                ProjectId = projectId,
                Status = TaskStatusEnum.ToDo,
                WorkType = WorkType.Task,
                Priority = Priority.Medium
            };

            var userIdString = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int userId))
                task.ReporterId = userId;

            ViewBag.Users = await _userService.GetAllUsersAsync();
            ViewBag.ProjectName = project.Name;

            return View(task);
        }

        // POST: Task/Create
        [HttpPost]
        public async Task<IActionResult> Create(TaskItem task)
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                if (task.ReporterId == 0)
                {
                    var userIdString = HttpContext.Session.GetString("UserId");
                    if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int userId))
                        task.ReporterId = userId;
                }

                task.CreatedAt = DateTime.Now;
                await _taskService.CreateTaskAsync(task);

                return RedirectToAction("Details", "Project", new { id = task.ProjectId });
            }

            ViewBag.Users = await _userService.GetAllUsersAsync();
            var project = await _projectService.GetProjectByIdAsync(task.ProjectId);
            ViewBag.ProjectName = project?.Name ?? "Unknown Project";

            return View(task);
        }

        // GET: Task/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
                return NotFound();

            return View(task);
        }

        // GET: Task/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
                return NotFound();

            ViewBag.Users = await _userService.GetAllUsersAsync();
            ViewBag.Projects = await _projectService.GetAllProjectsAsync();
            ViewBag.ProjectName = task.Project?.Name ?? "Unknown Project";

            return View(task);
        }

        // POST: Task/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, TaskItem task)
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            if (id != task.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                if (task.Status == TaskStatusEnum.Done && task.CompletionDate == null)
                    task.CompletionDate = DateTime.Now;

                await _taskService.UpdateTaskAsync(task);
                return RedirectToAction("Details", new { id = task.Id });
            }

            ViewBag.Users = await _userService.GetAllUsersAsync();
            ViewBag.Projects = await _projectService.GetAllProjectsAsync();
            var project = await _projectService.GetProjectByIdAsync(task.ProjectId);
            ViewBag.ProjectName = project?.Name ?? "Unknown Project";

            return View(task);
        }
    }
}
