using AgileBoard.Domain.Common;
using AgileBoard.Domain.Constants;
using AgileBoard.Domain.Entities;
using AgileBoard.Infrastructure.Repositories.Interfaces;
using AgileBoard.Services.Services.Interfaces;

namespace AgileBoard.Services.Services.Implementations
{
    public class ProjectService(IProjectRepository projectRepository) : IProjectService
    {
        private readonly IProjectRepository _projectRepository = projectRepository;

        public async Task<Result<IEnumerable<Project>>> GetAllProjectsAsync()
        {
            var projects = await _projectRepository.GetAllProjectsAsync();
            if (!projects.Any())
                return Result<IEnumerable<Project>>.NotFound(Messages.EntityNames.Projects, isPlural: true);

            return Result<IEnumerable<Project>>.Success(projects);
        }

        public async Task<Result<Project>> GetProjectByIdAsync(int id)
        {
            var project = await _projectRepository.GetProjectByIdAsync(id);
            if (project == null)
                return Result<Project>.NotFound(Messages.EntityNames.Project);

            return Result<Project>.Success(project);
        }

        public async Task<Result<Project>> GetProjectByNameAsync(string name)
        {
            var project = await _projectRepository.GetProjectByNameAsync(name);
            if (project == null)
                return Result<Project>.NotFound(Messages.EntityNames.Project);

            return Result<Project>.Success(project);
        }

        public async Task<Result<IEnumerable<Project>>> GetProjectsByOwnerIdAsync(int ownerId)
        {
            var projects = await _projectRepository.GetProjectsByOwnerIdAsync(ownerId);
            return Result<IEnumerable<Project>>.Success(projects);
        }

        public async Task<Result<IEnumerable<Project>>> GetProjectsByParticipantIdAsync(int participantId)
        {
            var projects = await _projectRepository.GetProjectsByParticipantIdAsync(participantId);
            return Result<IEnumerable<Project>>.Success(projects);
        }

        public async Task<Result<Project>> CreateProjectAsync(string name, string description, int ownerId)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<Project>.BadRequest(Messages.Projects.ProjectNameRequired);

            var existingProject = await _projectRepository.GetProjectByNameAsync(name);
            if (existingProject != null)
                return Result<Project>.Conflict(Messages.Projects.ProjectNameExists(name));

            try
            {
                var newProject = new Project
                {
                    Name = name,
                    Description = description,
                    OwnerId = ownerId,
                    CreationDate = DateTime.UtcNow
                };

                var createdProject = await _projectRepository.AddProjectAsync(newProject);
                return Result<Project>.Success(createdProject);
            }
            catch (Exception)
            {
                return Result<Project>.Failure(Messages.Generic.InternalServerError);
            }
        }

        public async Task<Result<Project>> UpdateProjectAsync(int id, string? name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(description))
                return Result<Project>.BadRequest(Messages.ProjectUpdate.NoFieldsSpecified);

            try
            {
                var updatedProject = await _projectRepository.UpdateProjectAsync(id, name, description);
                if (updatedProject == null)
                    return Result<Project>.NotFound(Messages.EntityNames.Project);

                return Result<Project>.Success(updatedProject);
            }
            catch (Exception)
            {
                return Result<Project>.Failure(Messages.Generic.InternalServerError);
            }
        }

        public async Task<Result<bool>> DeleteProjectAsync(int id)
        {
            var project = await _projectRepository.GetProjectByIdAsync(id);
            if (project == null)
                return Result<bool>.NotFound(Messages.EntityNames.Project);

            try
            {
                var deleted = await _projectRepository.DeleteProjectAsync(id);
                return Result<bool>.Success(deleted);
            }
            catch (Exception)
            {
                return Result<bool>.Failure(Messages.Generic.InternalServerError);
            }
        }

        public async Task<Result<bool>> AddParticipantAsync(int projectId, int userId)
        {
            try
            {
                var added = await _projectRepository.AddParticipantToProjectAsync(projectId, userId);
                if (!added)
                    return Result<bool>.BadRequest(Messages.Participants.AddParticipantFailed);

                return Result<bool>.Success(true);
            }
            catch (Exception)
            {
                return Result<bool>.Failure(Messages.Generic.InternalServerError);
            }
        }

        public async Task<Result<bool>> RemoveParticipantAsync(int projectId, int userId)
        {
            try
            {
                var removed = await _projectRepository.RemoveParticipantFromProjectAsync(projectId, userId);
                if (!removed)
                    return Result<bool>.BadRequest(Messages.Participants.RemoveParticipantFailed);

                return Result<bool>.Success(true);
            }
            catch (Exception)
            {
                return Result<bool>.Failure(Messages.Generic.InternalServerError);
            }
        }
    }
}
