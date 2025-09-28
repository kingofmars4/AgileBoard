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
            CreateMap<Project, ProjectDTO>();
            CreateMap<CreateProjectDTO, Project>();

            // TAGS
            CreateMap<Tag, TagDTO>();
            CreateMap<CreateTagDTO, Tag>();
            CreateMap<UpdateTagDTO, Tag>();
        }
    }
}
