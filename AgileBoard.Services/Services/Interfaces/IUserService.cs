using AgileBoard.Domain.Entities;

namespace AgileBoard.Services.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByUsernameAsync(string username);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> RegisterUserAsync(string username, string email, string password);
        Task<bool> VerifyLoginAsync(string username, string password);
        Task<User> UpdateUserAsync(int id, string? username, string? email);
        Task<bool> ChangePasswordAsync(int id, string currentPassword, string newPassword);
    }
}
