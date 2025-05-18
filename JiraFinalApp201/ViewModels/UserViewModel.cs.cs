using System.Collections.Generic;
using JiraFinalApp201.Models.Tasks;
using static JiraFinalApp201.Models.Tasks.Taskitems;

namespace JiraFinalApp201.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string FullName => Username;  

        // Task counts
        public int AssignedTasksCount { get; set; }
        public int ReportedTasksCount { get; set; }

        // Task lists if needed
        public IEnumerable<TaskViewModel> AssignedTasks { get; set; } = new List<TaskViewModel>();
        public IEnumerable<TaskViewModel> ReportedTasks { get; set; } = new List<TaskViewModel>();
    }
}