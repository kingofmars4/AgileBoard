using AgileBoard.API.DTOs;
using AgileBoard.Domain.Entities;
using AutoMapper;

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
                .ForMember(dest => dest.OwnerUsername, opt => opt.MapFrom(src => src.Owner!.Username))
                .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants));
            
            CreateMap<Project, ProjectSummaryDTO>()
                .ForMember(dest => dest.OwnerUsername, opt => opt.MapFrom(src => src.Owner!.Username))
                .ForMember(dest => dest.ParticipantCount, opt => opt.MapFrom(src => src.Participants != null ? src.Participants.Count() : 0));
            
            CreateMap<CreateProjectDTO, Project>();
            CreateMap<UpdateProjectDTO, Project>();

            // TAGS
            CreateMap<Tag, TagDTO>();
            CreateMap<CreateTagDTO, Tag>();
            CreateMap<UpdateTagDTO, Tag>();
        }
    }
}
