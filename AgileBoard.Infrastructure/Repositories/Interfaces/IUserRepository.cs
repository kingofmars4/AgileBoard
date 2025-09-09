using AgileBoard.Domain.Entities;

namespace AgileBoard.Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> AddUserAsync(User newUser);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUserameAsync(string username);
        Task<IEnumerable<User>> GetAllUsersAsync();

    }
}
