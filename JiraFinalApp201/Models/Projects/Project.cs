using JiraFinalApp201.Models.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static JiraFinalApp201.Models.Tasks.Taskitems;
using static JiraFinalApp201.Models.Tasks.Taskitems;
namespace JiraFinalApp201.Models.Projects
{
    public enum ProjectStatus
    {
        Active,
        Completed,
        OnHold
    }

    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public ProjectStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? EndDate { get; set; }

        // Navigation property for tasks related to this project
        public virtual ICollection<TaskItem> Tasks { get; set; } = new HashSet<TaskItem>();

        // Computed property to return number of tasks in this project
        public int TaskCount => Tasks?.Count() ?? 0;

        // Computed property to return number of completed tasks in this project
        public int CompletedTaskCount => Tasks?.Count(t => t.Status == TaskStatusEnum.Done) ?? 0;

        // Display string for status
        public string StatusDisplay => Status.ToString();

        public Project()
        {
            CreatedDate = DateTime.Now;
            Status = ProjectStatus.Active;  // Default
            Name = string.Empty;
            Description = string.Empty;
        }
    }
}
