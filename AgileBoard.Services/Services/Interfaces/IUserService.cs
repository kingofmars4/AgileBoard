using AgileBoard.Domain.Common;
using AgileBoard.Domain.Entities;

namespace AgileBoard.Services.Services.Interfaces
{
    public interface IUserService
    {
        Task<Result<User>> GetUserByIdAsync(int id);
        Task<Result<User>> GetUserByUsernameAsync(string username);
        Task<Result<IEnumerable<User>>> GetAllUsersAsync();
        Task<Result<User>> RegisterUserAsync(string username, string email, string password);
        Task<Result> VerifyLoginAsync(string username, string password);
        Task<Result<User>> UpdateUserAsync(int id, string? username, string? email);
        Task<Result> ChangePasswordAsync(int id, string currentPassword, string newPassword);
    }
}
