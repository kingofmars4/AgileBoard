using AgileBoard.Domain.Common;
using AgileBoard.Domain.Entities;

namespace AgileBoard.Services.Services.Interfaces
{
    public interface IProjectService
    {
        Task<Result<IEnumerable<Project>>> GetAllProjectsAsync();
        Task<Result<Project>> GetProjectByIdAsync(int id);
        Task<Result<Project>> GetProjectByNameAsync(string name);
        Task<Result<IEnumerable<Project>>> GetProjectsByOwnerIdAsync(int ownerId);
        Task<Result<IEnumerable<Project>>> GetProjectsByParticipantIdAsync(int participantId);
        Task<Result<Project>> CreateProjectAsync(string name, string description, int ownerId);
        Task<Result<Project>> UpdateProjectAsync(int id, string? name, string? description);
        Task<Result<bool>> DeleteProjectAsync(int id);
        Task<Result<bool>> AddParticipantAsync(int projectId, int userId);
        Task<Result<bool>> RemoveParticipantAsync(int projectId, int userId);
    }
}
