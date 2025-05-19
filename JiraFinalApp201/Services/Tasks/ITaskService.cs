using static JiraFinalApp201.Models.Tasks.Taskitems;

namespace JiraFinalApp201.Services.Tasks
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<TaskItem?> GetTaskByIdAsync(int id);
        // Retrieve task by its unique CON-Id (e.g., PROJ-123)
        Task<TaskItem?> GetTaskByConIdAsync(string conId);
        Task<IEnumerable<TaskItem>> GetTasksByProjectIdAsync(int projectId);
        Task<IEnumerable<TaskItem>> GetTasksByAssigneeIdAsync(int assigneeId);
        Task<IEnumerable<TaskItem>> GetTasksByReporterIdAsync(int reporterId);

        Task<TaskItem> CreateTaskAsync(TaskItem task);
        Task<TaskItem?> UpdateTaskAsync(TaskItem task);
        Task<bool> DeleteTaskAsync(int id);
        Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId);
        Task<TaskItem> AddTaskAsync(TaskItem task);
        
        // Find tasks matching the search term in any relevant field
        Task<IEnumerable<TaskItem>> SearchTasksAsync(string searchTerm);
    }
}
