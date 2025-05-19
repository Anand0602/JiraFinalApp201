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
                // Always generate CONId if null or empty
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
                // This is an alias for CreateTaskAsync
                return await CreateTaskAsync(task);
            }
 
            private async Task<string> GenerateConIdForTaskAsync(int projectId)
            {
                // Get project name for prefix or use default
                string prefix = "TASK";
                var project = await _context.Projects.FindAsync(projectId);
                if (project != null && !string.IsNullOrEmpty(project.Name))
                {
                    // Use first 3-4 characters of project name as prefix
                    prefix = project.Name.Length <= 4 ? 
                             project.Name.ToUpper() : 
                             project.Name.Substring(0, 4).ToUpper();
                }
                
                // Count existing tasks for this project to generate sequential number
                int taskCount = await _context.Tasks.CountAsync(t => t.ProjectId == projectId);
                
                // Format: PREFIX-123 (e.g., PROJ-123)
                return $"{prefix}-{taskCount + 1}";
            }
        }

}

