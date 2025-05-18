using JiraFinalApp201.Models.Projects;
namespace JiraFinalApp201.DataTransferObjects
{
    public class ProjectDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string StatusDisplay { get; set; }
        public int TaskCount { get; set; }
        public int CompletedTaskCount { get; set; }
    }
}
