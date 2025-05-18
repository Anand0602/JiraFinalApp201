using System.Collections.Generic;
using JiraFinalApp201.Models.Projects;
using JiraFinalApp201.Models.Authentication;

namespace JiraFinalApp201.ViewModels
{
    public class BoardViewModel
    {
        public List<TaskViewModel> Tasks { get; set; } = new List<TaskViewModel>();
        public UserTasksViewModel UserTasksViewModel { get; set; } = new UserTasksViewModel();
    }

    public class UserTasksViewModel
    {
        public IEnumerable<User> Users { get; set; } = new List<User>();
        public IEnumerable<TaskViewModel> Tasks { get; set; } = new List<TaskViewModel>();
        public IEnumerable<Project> Projects { get; set; } = new List<Project>();
    }
}