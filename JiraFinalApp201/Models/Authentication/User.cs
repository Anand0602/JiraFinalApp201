using JiraFinalApp201.Models.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static JiraFinalApp201.Models.Tasks.Taskitems;

namespace JiraFinalApp201.Models.Authentication
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; } // Plain text (not recommended for production)

        // Navigation properties
        public virtual ICollection<Taskitems.TaskItem> AssignedTasks { get; set; } = new HashSet<Taskitems.TaskItem>();
        public virtual ICollection<Taskitems.TaskItem> ReportedTasks { get; set; } = new HashSet<Taskitems.TaskItem>();

        // Computed properties to match UserDTO counts
        public int AssignedTasksCount => AssignedTasks?.Count() ?? 0;
        public int ReportedTasksCount => ReportedTasks?.Count() ?? 0;
        public User()
        {
            AssignedTasks = new HashSet<Taskitems.TaskItem>();
            ReportedTasks = new HashSet<Taskitems.TaskItem>();
        }
    }
}
