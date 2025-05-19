using Microsoft.AspNetCore.Mvc;
using JiraFinalApp201.Models.Tasks;
using JiraFinalApp201.Services.Tasks;
using JiraFinalApp201.Services.Projects;
using JiraFinalApp201.Services.User;
using System.Text.Json;
using static JiraFinalApp201.Models.Tasks.Taskitems;

namespace JiraFinalApp201.Controllers
{
    public class CalendarController : BaseController
    {
        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;
        private readonly ILogger<CalendarController> _logger;

        public CalendarController(
            ITaskService taskService,
            IProjectService projectService,
            IUserService userService,
            ILogger<CalendarController> logger)
            : base(projectService)
        {
            _taskService = taskService;
            _projectService = projectService;
            _userService = userService;
            _logger = logger;
        }

        private int? GetCurrentUserId()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            return int.TryParse(userIdStr, out var userId) ? userId : null;
        }

        // GET: Calendar
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // Get all tasks for the current user
            var tasks = await _taskService.GetAllTasksAsync();
            
            // Get all projects for filtering
            var projects = await _projectService.GetAllProjectsAsync();
            ViewBag.Projects = projects;
            
            return View(tasks);
        }

        // GET: Calendar/GetEvents
        [HttpGet]
        public async Task<IActionResult> GetEvents(DateTime start, DateTime end)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Json(new { success = false, message = "User not authenticated" });

            try
            {
                // Get all tasks
                var tasks = await _taskService.GetAllTasksAsync();
                
                // Convert tasks to calendar events
                var events = tasks.Select(task => new
                {
                    id = task.Id,
                    title = !string.IsNullOrEmpty(task.Title) ? task.Title : "Untitled Task",
                    start = task.CreatedAt.ToString("yyyy-MM-dd"),
                    end = task.CompletionDate?.ToString("yyyy-MM-dd") ?? task.CreatedAt.AddDays(1).ToString("yyyy-MM-dd"),
                    allDay = true,
                    backgroundColor = GetColorForStatus(task.Status),
                    borderColor = GetColorForStatus(task.Status),
                    textColor = "#ffffff",
                    description = task.Description ?? "",
                    status = task.Status.ToString(),
                    priority = task.Priority.ToString(),
                    projectId = task.ProjectId,
                    projectName = task.Project?.Name ?? "Unknown Project",
                    assigneeId = task.AssigneeId,
                    assigneeName = task.Assignee?.Username ?? "Unassigned",
                    url = Url.Action("Details", "Task", new { id = task.Id })
                }).ToList();

                return Json(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting calendar events");
                return Json(new { success = false, message = "Error getting calendar events" });
            }
        }

        private string GetColorForStatus(TaskStatusEnum status)
        {
            return status switch
            {
                TaskStatusEnum.ToDo => "#0d6efd", // Blue
                TaskStatusEnum.InProgress => "#ffc107", // Yellow
                TaskStatusEnum.Done => "#198754", // Green
                _ => "#6c757d" // Gray
            };
        }
    }
}
