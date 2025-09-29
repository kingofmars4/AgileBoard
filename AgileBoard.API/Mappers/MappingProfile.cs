using AgileBoard.API.DTOs;
using AgileBoard.Domain.Entities;
using AutoMapper;
using static AgileBoard.Domain.Entities.WorkItem;

namespace AgileBoard.API.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // USERS
            CreateMap<User, UserDTO>();
            CreateMap<CreateUserDTO, User>();

            // PROJECTS
            CreateMap<Project, ProjectDTO>()
                .ForMember(dest => dest.OwnerUsername, opt => opt.MapFrom(src => src.Owner != null ? src.Owner.Username : "Unknown"))
                .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants ?? new List<User>()));
            
            CreateMap<Project, ProjectSummaryDTO>()
                .ForMember(dest => dest.OwnerUsername, opt => opt.MapFrom(src => src.Owner != null ? src.Owner.Username : "Unknown"))
                .ForMember(dest => dest.ParticipantCount, opt => opt.MapFrom(src => src.Participants != null ? src.Participants.Count() : 0));
            
            CreateMap<CreateProjectDTO, Project>();
            CreateMap<UpdateProjectDTO, Project>();

            // TAGS
            CreateMap<Tag, TagDTO>();
            CreateMap<CreateTagDTO, Tag>();
            CreateMap<UpdateTagDTO, Tag>();

            // WORKITEMS
            CreateMap<WorkItem, WorkItemDTO>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : "Unknown"))
                .ForMember(dest => dest.SprintName, opt => opt.MapFrom(src => src.Sprint != null ? src.Sprint.Name : null))
                .ForMember(dest => dest.AssignedUsers, opt => opt.MapFrom(src => src.AssignedUsers ?? new List<User>()))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags ?? new List<Tag>()));

            CreateMap<WorkItem, WorkItemSummaryDTO>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.AssignedUsersCount, opt => opt.MapFrom(src => src.AssignedUsers != null ? src.AssignedUsers.Count() : 0))
                .ForMember(dest => dest.TagsCount, opt => opt.MapFrom(src => src.Tags != null ? src.Tags.Count() : 0));

            CreateMap<CreateWorkItemDTO, WorkItem>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State));

            CreateMap<UpdateWorkItemDTO, WorkItem>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State.HasValue ? src.State.Value : WorkItemState.ToDo));
        }
    }
}
