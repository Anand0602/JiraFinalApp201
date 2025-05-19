using Microsoft.AspNetCore.Mvc;
using JiraFinalApp201.Models.Tasks;
using static JiraFinalApp201.Models.Tasks.Taskitems;
using JiraFinalApp201.Models.Database;
using JiraFinalApp201.Services.Tasks;
using JiraFinalApp201.Services.Projects;
using JiraFinalApp201.Services.User;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace JiraFinalApp201.Controllers
{
    
   public class TaskController : BaseController
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
            : base(projectService)
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
                return BadRequest("ProjectId is required.");

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskItem task, IFormFile Attachment)
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            // Always remove CONId validation errors since we'll handle it in the service
            ModelState.Remove("CONId");
            
            // Ensure CONId is null if empty string to trigger auto-generation
            if (string.IsNullOrWhiteSpace(task.CONId))
            {
                task.CONId = null;
            }

            if (ModelState.IsValid)
            {
                if (task.ReporterId == 0)
                {
                    var userIdString = HttpContext.Session.GetString("UserId");
                    if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int userId))
                        task.ReporterId = userId;
                }

                task.CreatedAt = DateTime.Now;
                var createdTask = await _taskService.CreateTaskAsync(task);

                // Handle attachment upload if provided
                if (Attachment != null && Attachment.Length > 0)
                {
                    // Only allow image files
                    string fileExtension = Path.GetExtension(Attachment.FileName).ToLower();
                    if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif")
                    {
                        // Create uploads directory if it doesn't exist
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        // Generate unique filename
                        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(Attachment.FileName)}";
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        // Save file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await Attachment.CopyToAsync(stream);
                        }

                        // Update task with attachment path
                        createdTask.AttachmentPath = $"/uploads/{fileName}";
                        await _taskService.UpdateTaskAsync(createdTask);
                    }
                    else
                    {
                        TempData["AttachmentError"] = "Only image files (jpg, jpeg, png, gif) are allowed.";
                    }
                }

                // Redirect to dashboard (Board/Index) with success parameter
                TempData["TaskCreated"] = true;
                TempData["TaskTitle"] = createdTask.Title;
                return RedirectToAction("Index", "Board", new { created = true });
            }

            // Even if validation fails, try to create the task anyway
            try
            {
                if (task.ReporterId == 0)
                {
                    var userIdString = HttpContext.Session.GetString("UserId");
                    if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int userId))
                        task.ReporterId = userId;
                }

                task.CreatedAt = DateTime.Now;
                var createdTask = await _taskService.CreateTaskAsync(task);
                
                // Handle attachment upload if provided
                if (Attachment != null && Attachment.Length > 0)
                {
                    // Only allow image files
                    string fileExtension = Path.GetExtension(Attachment.FileName).ToLower();
                    if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif")
                    {
                        // Create uploads directory if it doesn't exist
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        // Generate unique filename
                        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(Attachment.FileName)}";
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        // Save file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await Attachment.CopyToAsync(stream);
                        }

                        // Update task with attachment path
                        createdTask.AttachmentPath = $"/uploads/{fileName}";
                        await _taskService.UpdateTaskAsync(createdTask);
                    }
                    else
                    {
                        TempData["AttachmentError"] = "Only image files (jpg, jpeg, png, gif) are allowed.";
                    }
                }
                
                TempData["TaskCreated"] = true;
                TempData["TaskTitle"] = createdTask.Title;
            }
            catch (Exception ex)
            {
                // Log the error but still redirect to Board
                TempData["TaskError"] = "There was an error creating the task: " + ex.Message;
            }
            
            // Always redirect to Board page
            return RedirectToAction("Index", "Board");
        }

        // GET: Task/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
                return NotFound();
            
            // Add user and project information for the view
            ViewBag.Users = await _userService.GetAllUsersAsync();
            ViewBag.ProjectName = task.Project?.Name ?? "Unknown Project";
            ViewBag.ReporterName = task.Reporter?.Username ?? "Unknown";
            ViewBag.AssigneeName = task.Assignee?.Username ?? null;

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
        
        // GET: Task/GetTaskDetails
        public async Task<IActionResult> GetTaskDetails(int taskId)
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return Json(new { error = "User not authenticated" });

            try
            {
                var task = await _taskService.GetTaskByIdAsync(taskId);
                if (task == null)
                    return NotFound();

                // Set ViewBag for task details page
                ViewBag.IsTaskDetailsPage = true;
                
                // Return the Details view for the task
                return View("Details", task);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // POST: Task/ChangeTaskStatus
        [HttpPost]
        public async Task<IActionResult> ChangeTaskStatus(int taskId, int newStatus)
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return Json(new { success = false, message = "User not authenticated" });

            try
            {
                var task = await _taskService.GetTaskByIdAsync(taskId);
                if (task == null)
                    return Json(new { success = false, message = "Task not found" });

                // Convert the integer newStatus to the enum value
                if (!Enum.IsDefined(typeof(TaskStatusEnum), newStatus))
                    return Json(new { success = false, message = "Invalid status value" });

                task.Status = (TaskStatusEnum)newStatus;
                
                // Set completion date if status is Done
                if (task.Status == TaskStatusEnum.Done && task.CompletionDate == null)
                    task.CompletionDate = DateTime.Now;
                // Clear completion date if status is not Done
                else if (task.Status != TaskStatusEnum.Done)
                    task.CompletionDate = null;

                await _taskService.UpdateTaskAsync(task);
                
                return Json(new { success = true, message = "Task status updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        
        // POST: Task/UploadAttachment
        [HttpPost]
        public async Task<IActionResult> UploadAttachment(int taskId, IFormFile attachment)
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            if (attachment == null || attachment.Length == 0)
                return RedirectToAction("Details", new { id = taskId });

            var task = await _taskService.GetTaskByIdAsync(taskId);
            if (task == null)
                return NotFound();

            // Remove old attachment if exists
            if (!string.IsNullOrEmpty(task.AttachmentPath))
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", task.AttachmentPath.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    try
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                    catch (Exception)
                    {
                        // Log error but continue
                    }
                }
            }

            // Create uploads directory if it doesn't exist
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(attachment.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await attachment.CopyToAsync(stream);
            }

            // Update task with new attachment path
            task.AttachmentPath = $"/uploads/{fileName}";
            await _taskService.UpdateTaskAsync(task);

            return RedirectToAction("Details", new { id = taskId });
        }

        // POST: Task/RemoveAttachment
        [HttpPost]
        public async Task<IActionResult> RemoveAttachment(int taskId)
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            var task = await _taskService.GetTaskByIdAsync(taskId);
            if (task == null)
                return NotFound();

            if (!string.IsNullOrEmpty(task.AttachmentPath))
            {
                // Delete file from server
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", task.AttachmentPath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception)
                    {
                        // Log error but continue
                    }
                }

                // Clear attachment path in database
                task.AttachmentPath = null;
                await _taskService.UpdateTaskAsync(task);
            }

            return RedirectToAction("Details", new { id = taskId });
        }
        
        // POST: Task/UpdateField
        [HttpPost]
        public async Task<IActionResult> UpdateField(int id, string field, string value)
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return Json(new { success = false, message = "User not authenticated" });
            
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
                return Json(new { success = false, message = "Task not found" });
            
            try
            {
                switch (field)
                {
                    case "Title":
                        task.Title = value;
                        break;
                    case "Description":
                        task.Description = value;
                        break;
                    case "Status":
                        if (Enum.TryParse<TaskStatusEnum>(value, out var status))
                        {
                            task.Status = status;
                            
                            // If status is changed to Done, set completion date
                            if (status == TaskStatusEnum.Done && task.CompletionDate == null)
                                task.CompletionDate = DateTime.Now;
                            // If status is changed from Done, clear completion date
                            else if (status != TaskStatusEnum.Done)
                                task.CompletionDate = null;
                        }
                        break;
                    case "Priority":
                        if (Enum.TryParse<Priority>(value, out var priority))
                            task.Priority = priority;
                        break;
                    case "AssigneeId":
                        if (string.IsNullOrEmpty(value))
                            task.AssigneeId = null;
                        else if (int.TryParse(value, out var assigneeId))
                            task.AssigneeId = assigneeId;
                        break;
                    default:
                        return Json(new { success = false, message = "Invalid field" });
                }
                
                await _taskService.UpdateTaskAsync(task);
                
                // Return updated display value
                string displayValue = value;
                if (field == "Status" || field == "Priority")
                    displayValue = value; // Already in display format
                else if (field == "AssigneeId")
                {
                    if (string.IsNullOrEmpty(value) || value == "0")
                        displayValue = "Unassigned";
                    else
                    {
                        var user = await _userService.GetUserByIdAsync(int.Parse(value));
                        displayValue = user?.Username ?? "Unknown";
                    }
                }
                
                return Json(new { success = true, displayValue });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
