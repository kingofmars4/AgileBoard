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
            CreateMap<CreateUserDTO, User>();
            CreateMap<User, UserDTO>();

            // PROJECTS
            CreateMap<Project, ProjectDTO>();
            CreateMap<CreateProjectDTO, Project>();

        }
    }
}
