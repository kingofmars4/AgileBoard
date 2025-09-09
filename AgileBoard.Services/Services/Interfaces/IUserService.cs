using AgileBoard.Domain.Entities;

namespace AgileBoard.Services.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> RegisterUserAsync(string username, string email, string password);
        Task<bool> VerifiyLoginAsync(string username, string password);
    }
}
