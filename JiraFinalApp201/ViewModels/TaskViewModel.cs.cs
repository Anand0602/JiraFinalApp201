using System;
using System.Collections.Generic;
using JiraFinalApp201.Models.Authentication;
using JiraFinalApp201.Models.Projects;
using static JiraFinalApp201.Models.Tasks.Taskitems;

namespace JiraFinalApp201.ViewModels
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskStatusEnum Status { get; set; }
        public WorkType WorkType { get; set; }
        public Priority Priority { get; set; }
        public string CONId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string AttachmentPath { get; set; }

        // Foreign keys
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }

        public int ReporterId { get; set; }
        public string ReporterName { get; set; }

        public int? AssigneeId { get; set; }
        public string AssigneeName { get; set; }

        // Navigation properties for dropdowns in forms
        public IEnumerable<User> Users { get; set; } = new List<User>();
        public IEnumerable<Project> Projects { get; set; } = new List<Project>();
    }
}