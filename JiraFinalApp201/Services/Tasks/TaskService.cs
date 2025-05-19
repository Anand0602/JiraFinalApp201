using static JiraFinalApp201.Models.Tasks.Taskitems;
using JiraFinalApp201.Models.Database;
using JiraFinalApp201 .Models.Tasks;
using JiraFinalApp201.Services.Tasks;
using Microsoft.EntityFrameworkCore;

namespace JiraFinalApp201.Services.Tasks
{
   
        public class TaskService : ITaskService
        {
            private readonly JiraFinalApp201Db _context;

            public TaskService(JiraFinalApp201Db context)
            {
                _context = context;
            }

            public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
            {
                return await _context.Tasks
                    .Include(t => t.Assignee)
                    .Include(t => t.Reporter)
                    .Include(t => t.Project)
                    .ToListAsync();
            }

            public async Task<TaskItem?> GetTaskByIdAsync(int id)
            {
                return await _context.Tasks
                    .Include(t => t.Assignee)
                    .Include(t => t.Reporter)
                    .Include(t => t.Project)
                    .FirstOrDefaultAsync(t => t.Id == id);
            }

            public async Task<TaskItem?> GetTaskByConIdAsync(string conId)
            {
                return await _context.Tasks
                    .Include(t => t.Assignee)
                    .Include(t => t.Reporter)
                    .Include(t => t.Project)
                    .FirstOrDefaultAsync(t => t.CONId == conId);
            }

            public async Task<TaskItem> CreateTaskAsync(TaskItem task)
            {
                // Auto-generate task ID if not provided
                if (string.IsNullOrWhiteSpace(task.CONId))
                {
                    task.CONId = await GenerateConIdForTaskAsync(task.ProjectId);
                }

                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();
                return task;
            }

            public async Task<TaskItem?> UpdateTaskAsync(TaskItem task)
            {
                var existingTask = await _context.Tasks.FindAsync(task.Id);
                if (existingTask == null) return null;

                _context.Entry(existingTask).CurrentValues.SetValues(task);
                await _context.SaveChangesAsync();
                return existingTask;
            }

            public async Task<bool> DeleteTaskAsync(int id)
            {
                var task = await _context.Tasks.FindAsync(id);
                if (task == null)
                    return false;

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
                return true;
            }

            public async Task<IEnumerable<TaskItem>> GetTasksByProjectIdAsync(int projectId)
            {
                return await _context.Tasks
                    .Include(t => t.Assignee)
                    .Include(t => t.Reporter)
                    .Where(t => t.ProjectId == projectId)
                    .ToListAsync();
            }

            public async Task<IEnumerable<TaskItem>> GetTasksByAssigneeIdAsync(int assigneeId)
            {
                return await _context.Tasks
                    .Include(t => t.Project)
                    .Include(t => t.Reporter)
                    .Where(t => t.AssigneeId == assigneeId)
                    .ToListAsync();
            }

            public async Task<IEnumerable<TaskItem>> GetTasksByReporterIdAsync(int reporterId)
            {
                return await _context.Tasks
                    .Include(t => t.Project)
                    .Include(t => t.Assignee)
                    .Where(t => t.ReporterId == reporterId)
                    .ToListAsync();
            }

            public async Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId)
            {
                return await _context.Tasks
                    .Include(t => t.Assignee)
                    .Include(t => t.Reporter)
                    .Include(t => t.Project)
                    .Where(t => t.AssigneeId == userId || t.ReporterId == userId)
                    .ToListAsync();
            }

            public async Task<TaskItem> AddTaskAsync(TaskItem task)
            {
                // Wrapper for CreateTaskAsync for backward compatibility
                return await CreateTaskAsync(task);
            }
 
            private async Task<string> GenerateConIdForTaskAsync(int projectId)
            {
                // Default prefix if no project found
                string prefix = "TASK";
                var project = await _context.Projects.FindAsync(projectId);
                if (project != null && !string.IsNullOrEmpty(project.Name))
                {
                    // Take first few chars of project name for the prefix
                    prefix = project.Name.Length <= 4 ? 
                             project.Name.ToUpper() : 
                             project.Name.Substring(0, 4).ToUpper();
                }
                
                // Get next number in sequence for this project
                int taskCount = await _context.Tasks.CountAsync(t => t.ProjectId == projectId);
                
                // Create ID in format: PROJ-123
                return $"{prefix}-{taskCount + 1}";
            }
            
            public async Task<IEnumerable<TaskItem>> SearchTasksAsync(string searchTerm)
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return new List<TaskItem>();
                
                searchTerm = searchTerm.ToLower().Trim();
                
                return await _context.Tasks
                    .Include(t => t.Assignee)
                    .Include(t => t.Reporter)
                    .Include(t => t.Project)
                    .Where(t => 
                        (t.CONId != null && t.CONId.ToLower().Contains(searchTerm)) ||
                        (t.Title != null && t.Title.ToLower().Contains(searchTerm)) ||
                        (t.Description != null && t.Description.ToLower().Contains(searchTerm)) ||
                        (t.Assignee != null && t.Assignee.Username != null && t.Assignee.Username.ToLower().Contains(searchTerm)) ||
                        (t.Reporter != null && t.Reporter.Username != null && t.Reporter.Username.ToLower().Contains(searchTerm))
                    )
                    .OrderByDescending(t => t.CreatedAt)
                    .Take(10) // Cap results to avoid performance issues
                    .ToListAsync();
            }
        }

}
