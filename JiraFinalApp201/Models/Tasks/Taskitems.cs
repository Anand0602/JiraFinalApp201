using JiraFinalApp201.Models.Authentication;
using JiraFinalApp201.Models.Projects;
using System;
using System.ComponentModel.DataAnnotations;

namespace JiraFinalApp201.Models.Tasks
{
    public class Taskitems
    {
        // Enum for task statuses
        public enum TaskStatusEnum
        {
            ToDo,
            InProgress,
            Done
        }

        // Enum for work types
        public enum WorkType
        {
            Story,
            Bug,
            Task
        }

        // Enum for priority levels
        public enum Priority
        {
            Low,
            Medium,
            High
        }

        // Nested TaskItem class
        public class TaskItem
        {
            public int Id { get; set; }

            [Required]
            [StringLength(100)]
            public string Title { get; set; }

            public string Description { get; set; }

            public TaskStatusEnum Status { get; set; }

            public WorkType WorkType { get; set; }

            public Priority Priority { get; set; }

            public DateTime CreatedAt { get; set; }

            public DateTime? CompletionDate { get; set; }

            [MaxLength(50)]
            public string? CONId { get; set; }

            public string? AttachmentPath { get; set; }

            // Foreign keys and navigation properties
            public int ProjectId { get; set; }
            public virtual Project Project { get; set; }

            public int ReporterId { get; set; }
            public virtual User Reporter { get; set; }

            public int? AssigneeId { get; set; }
            public virtual User Assignee { get; set; }

            // Constructor to initialize default values
            public TaskItem()
            {
                Title = "";
                Description = "";
                Status = TaskStatusEnum.ToDo;
                WorkType = WorkType.Task;
                Priority = Priority.Medium;
                CreatedAt = DateTime.Now;
            }
        }
    }
}
