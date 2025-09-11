using AgileBoard.Domain.Entities;

namespace AgileBoard.Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> AddUserAsync(User newUser);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUserameAsync(string username);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> UpdateUserAsync(int id, string? username, string? email);
        Task<bool> ChangePasswordAsync(int id, string newPasswordHash, byte[] newPasswordSalt);

    }
}
