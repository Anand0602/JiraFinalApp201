using static JiraFinalApp201.Models.Tasks.Taskitems;

namespace JiraFinalApp201.Services.Tasks
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<TaskItem?> GetTaskByIdAsync(int id);
        Task<TaskItem?> GetTaskByConIdAsync(string conId); //   CON-Id
        Task<IEnumerable<TaskItem>> GetTasksByProjectIdAsync(int projectId);
        Task<IEnumerable<TaskItem>> GetTasksByAssigneeIdAsync(int assigneeId);
        Task<IEnumerable<TaskItem>> GetTasksByReporterIdAsync(int reporterId);

        Task<TaskItem> CreateTaskAsync(TaskItem task);
        Task<TaskItem?> UpdateTaskAsync(TaskItem task);
        Task<bool> DeleteTaskAsync(int id);
        Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId);
        Task<TaskItem> AddTaskAsync(TaskItem task);

    }
}
