using AgileBoard.Domain.Entities;

namespace AgileBoard.Infrastructure.Repositories.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project> AddProjectAsync(Project project);
        Task<Project?> GetProjectByIdAsync(int id);
        Task<Project?> GetProjectByNameAsync(string name);
        Task<IEnumerable<Project>> GetAllProjectsAsync();
        Task<IEnumerable<Project>> GetProjectsByOwnerIdAsync(int ownerId);
        Task<IEnumerable<Project>> GetProjectsByParticipantIdAsync(int participantId);
        Task<Project?> UpdateProjectAsync(int id, string? name, string? description);
        Task<bool> DeleteProjectAsync(int id);
        Task<bool> AddParticipantToProjectAsync(int projectId, int userId);
        Task<bool> RemoveParticipantFromProjectAsync(int projectId, int userId);
    }
}
