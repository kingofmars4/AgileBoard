using AgileBoard.Domain.Common;
using AgileBoard.Domain.Constants;
using AgileBoard.Infrastructure.Repositories.Interfaces;
using AgileBoard.Services.Security.Interfaces;

namespace AgileBoard.Services.Security.Implementations
{
#pragma warning disable CS1998
    public class AuthorizationService(IUserRepository userRepository, IProjectRepository projectRepository)
        : IAuthorizationService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IProjectRepository _projectRepository = projectRepository;

        public async Task<Result<bool>> CanAccessProjectAsync(int userId, int projectId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return Result<bool>.Unauthorized(Messages.Generic.NotFound(Messages.EntityNames.User));

            var project = await _projectRepository.GetProjectByIdAsync(projectId);
            if (project == null)
                return Result<bool>.NotFound(Messages.EntityNames.Project);

            var canAccess = 
                project.OwnerId == userId || // User is owner
                (project.Participants?.Any(p => p.Id == userId) ?? false); // User is participant

            return Result<bool>.Success(canAccess);
        }

        public async Task<Result<bool>> CanModifyProjectAsync(int userId, int projectId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return Result<bool>.Unauthorized(Messages.Generic.NotFound(Messages.EntityNames.User));

            var project = await _projectRepository.GetProjectByIdAsync(projectId);
            if (project == null)
                return Result<bool>.NotFound(Messages.EntityNames.Project);

            var canModify = project.OwnerId == userId; // Only owner can modify

            return Result<bool>.Success(canModify);
        }

        public async Task<Result<bool>> CanModifyUserAsync(int currentUserId, int targetUserId)
        {
            var canModify = currentUserId == targetUserId; // Only Users can modify their own data
            
            return Result<bool>.Success(canModify);
        }
    }
}
