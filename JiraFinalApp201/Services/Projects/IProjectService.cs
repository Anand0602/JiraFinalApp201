using JiraFinalApp201.Models.Projects;
namespace JiraFinalApp201.Services.Projects
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetAllProjectsAsync();
        Task<Project> GetProjectByIdAsync(int id);
        Task<Project> CreateProjectAsync(Project project);
       
    }
}
