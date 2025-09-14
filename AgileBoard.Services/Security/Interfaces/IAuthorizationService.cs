using AgileBoard.Domain.Common;

namespace AgileBoard.Services.Security.Interfaces
{
    public interface IAuthorizationService
    {
        Task<Result<bool>> CanAccessProjectAsync(int userId, int projectId);
        Task<Result<bool>> CanModifyProjectAsync(int userId, int projectId);
        Task<Result<bool>> CanModifyUserAsync(int currentUserId, int targetUserId);
    }
}
