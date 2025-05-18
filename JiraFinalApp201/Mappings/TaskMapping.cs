using AutoMapper;
using JiraFinalApp201.Models.Tasks;
using JiraFinalApp201.ViewModels;
using static JiraFinalApp201.Models.Tasks.Taskitems;

namespace JiraApp.Mappings
{
    public class TaskMapping : Profile
    {
        public TaskMapping()
        {
            // Map from TaskItem to TaskViewModel and vice versa
            CreateMap<TaskItem, TaskViewModel>()
                .ForMember(dest => dest.AssigneeName, opt => opt.MapFrom(src => src.Assignee != null ? src.Assignee.Username : "Unassigned"))
                .ForMember(dest => dest.ReporterName, opt => opt.MapFrom(src => src.Reporter != null ? src.Reporter.Username : "Unknown"))
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : "No Project"));

            CreateMap<TaskViewModel, TaskItem>();
        }
    }
}
