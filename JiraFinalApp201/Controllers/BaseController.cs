using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using JiraFinalApp201.Services.Projects;

namespace JiraFinalApp201.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IProjectService _projectService;

        public BaseController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Get current user ID from session
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (userIdStr != null)
            {
                // Load projects for the sidebar
                var projects = await _projectService.GetAllProjectsAsync();
                ViewBag.Projects = projects;
                
               
                if (context.RouteData.Values.ContainsKey("projectId"))
                {
                    if (int.TryParse(context.RouteData.Values["projectId"].ToString(), out int projectId))
                    {
                        ViewBag.SelectedProjectId = projectId;
                    }
                }
                else if (context.HttpContext.Request.Query.ContainsKey("projectId"))
                {
                    if (int.TryParse(context.HttpContext.Request.Query["projectId"], out int projectId))
                    {
                        ViewBag.SelectedProjectId = projectId;
                    }
                }
            }

            await base.OnActionExecutionAsync(context, next);
        }

        protected int? GetCurrentUserId()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            return int.TryParse(userIdStr, out var userId) ? userId : null;
        }
    }
}
