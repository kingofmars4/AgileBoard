using AgileBoard.Domain.Entities;

namespace AgileBoard.Services.Services.Interfaces
{
    public interface IProjectService
    {
        Task<Project> CreateProjectAsync(Project newProject);
        Task<Project> GetProjectByIdAsync(int id);
    }
}
