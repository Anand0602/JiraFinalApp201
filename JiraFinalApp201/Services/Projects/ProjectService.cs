using JiraFinalApp201.Models.Database;
using JiraFinalApp201.Models.Projects;
using JiraFinalApp201.Services.Projects;
using Microsoft.EntityFrameworkCore;

namespace JiraFinalApp201.Services.Projects
{
    public class ProjectService : IProjectService
    {
        private readonly JiraFinalApp201Db _context;

        public ProjectService(JiraFinalApp201Db context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return await _context.Projects.ToListAsync();
        }

        public async Task<Project> GetProjectByIdAsync(int id)
        {
            return await _context.Projects.FindAsync(id);
        }

        public async Task<Project> CreateProjectAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

    }
}