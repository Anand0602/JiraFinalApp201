using Microsoft.AspNetCore.Mvc;
using JiraFinalApp201.Services.User;
using AutoMapper;
using JiraFinalApp201.Services.Tasks;
using static JiraFinalApp201.Models.Tasks.Taskitems;
using JiraFinalApp201.Services.Projects;
using JiraFinalApp201.Models.Authentication;
using JiraFinalApp201.Models.Projects;
using JiraFinalApp201.Models.Authentication;
using System.Collections.Generic;
using JiraFinalApp201.ViewModels;


namespace JiraFinalApp201.Controllers
{
    // View models defined directly in the controller file
    namespace JiraFinalApp201.ViewModels
    {
        public class TaskViewModel
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public TaskStatusEnum Status { get; set; }
            public WorkType WorkType { get; set; }
            public Priority Priority { get; set; }
            public string CONId { get; set; }
            public int ProjectId { get; set; }
            public string ProjectName { get; set; }
            public int? AssigneeId { get; set; }
            public string AssigneeName { get; set; }
            public int ReporterId { get; set; }
            public string ReporterName { get; set; }
        }

        public class BoardViewModel
        {
            public List<TaskViewModel> Tasks { get; set; }
            public UserTasksViewModel UserTasksViewModel { get; set; }
        }

        public class UserTasksViewModel
        {
            public IEnumerable<User> Users { get; set; }
            public IEnumerable<TaskViewModel> Tasks { get; set; }
            public IEnumerable<Project> Projects { get; set; }
        }
    }

    public class BoardController : BaseController
    {
        private readonly ILogger<BoardController> _logger;
        private readonly ITaskService _taskService;
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;
        private readonly IMapper _mapper;

        public BoardController(
            ILogger<BoardController> logger,
            ITaskService taskService,
            IUserService userService,
            IProjectService projectService,
            IMapper mapper)
            : base(projectService)
        {
            _logger = logger;
            _taskService = taskService;
            _userService = userService;
            _projectService = projectService;
            _mapper = mapper;
        }

        private int? GetCurrentUserId()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            return int.TryParse(userIdStr, out var userId) ? userId : null;
        }

        private bool IsAjaxRequest() =>
            Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        public async Task<IActionResult> Index(bool created = false)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");
                
            // Load all users
            var users = await _userService.GetAllUsersAsync();
            
            // Load all projects
            var projects = await _projectService.GetAllProjectsAsync();
            ViewBag.Projects = projects;
            
            // Load ALL tasks for dashboard counts - this is critical for accurate counts
            var allTasks = await _taskService.GetAllTasksAsync();
            var allMappedTasks = _mapper.Map<List<TaskViewModel>>(allTasks);
            
            // Create the view model with all tasks for proper counts
            var model = new BoardViewModel
            {
                Tasks = allMappedTasks,
                UserTasksViewModel = new UserTasksViewModel
                {
                    Users = users,
                    Tasks = allMappedTasks, // Use all tasks here too for consistency
                    Projects = projects
                }
            };
            
            // Set ViewBag variables for task creation success message
            ViewBag.TaskCreated = TempData["TaskCreated"];
            ViewBag.TaskTitle = TempData["TaskTitle"];

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveTask(TaskViewModel taskViewModel)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                if (IsAjaxRequest())
                    return PartialView("_CreateTaskModal", taskViewModel);

                return View("Index", new BoardViewModel { UserTasksViewModel = new UserTasksViewModel() });
            }

            var task = _mapper.Map<TaskItem>(taskViewModel);
            task.ReporterId = userId.Value;
            task.CreatedAt = DateTime.UtcNow;
            task.Status = task.Status == 0 ? TaskStatusEnum.ToDo : task.Status;

            await _taskService.AddTaskAsync(task);
            return RedirectToAction("Index", "Board", new { created = 1 });

        }

        public async Task<IActionResult> GetTaskDetails(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
                return NotFound();

            var taskViewModel = _mapper.Map<TaskViewModel>(task);
            return PartialView("_TaskDetailsPartial", taskViewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
                return NotFound();

            var taskViewModel = _mapper.Map<TaskViewModel>(task);
            taskViewModel.Users = await _userService.GetAllUsersAsync();
            taskViewModel.Projects = await _projectService.GetAllProjectsAsync();

            return View(taskViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TaskViewModel taskViewModel)
        {
            if (!ModelState.IsValid)
            {
                taskViewModel.Users = await _userService.GetAllUsersAsync();
                taskViewModel.Projects = await _projectService.GetAllProjectsAsync();
                return View(taskViewModel);
            }

            var task = _mapper.Map<TaskItem>(taskViewModel);
            await _taskService.UpdateTaskAsync(task);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
                return NotFound();

            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _taskService.DeleteTaskAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ViewBoard()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var tasks = await _taskService.GetTasksByUserIdAsync(userId.Value);

            var viewModel = new BoardViewModel
            {
                Tasks = _mapper.Map<List<TaskViewModel>>(tasks),
                UserTasksViewModel = new UserTasksViewModel
                {
                    Users = await _userService.GetAllUsersAsync(),
                    Projects = await _projectService.GetAllProjectsAsync()
                }
            };
            
            return View(viewModel);
        }
        
        [HttpGet]
        public async Task<IActionResult> Search(string term)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Json(new { success = false, message = "User not authenticated" });
                    
                if (string.IsNullOrWhiteSpace(term))
                    return Json(new { success = true, results = new List<object>() });
                    
                // Get search results
                var searchResults = await _taskService.SearchTasksAsync(term);
                
                // Transform to view model
                var results = searchResults.Select(task => new {
                    id = task.Id,
                    conId = task.CONId ?? $"TASK-{task.Id}",  // Fallback if CONId is null
                    title = task.Title ?? "Untitled Task",      // Fallback if Title is null
                    status = task.Status.ToString(),
                    statusClass = GetStatusClass(task.Status)
                }).ToList();
                
                // For debugging
                _logger.LogInformation($"Search for '{term}' returned {results.Count} results");
                
                return Json(new { 
                    success = true, 
                    results = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching for '{term}'");
                return Json(new { success = false, message = "An error occurred while searching." });
            }
        }
        
        private string GetStatusClass(TaskStatusEnum status)
        {
            return status switch
            {
                TaskStatusEnum.ToDo => "to-do",
                TaskStatusEnum.InProgress => "in-progress",
                TaskStatusEnum.Done => "done",
                _ => ""
            };
        }
    }
}