using AgileBoard.Domain.Entities;
using AgileBoard.Infrastructure.Repositories.Interfaces;
using AgileBoard.Services.Security.Interfaces;
using AgileBoard.Services.Services.Interfaces;

namespace AgileBoard.Services.Services.Implementations
{
    public class UserService(IUserRepository userRepository, IPasswordHasher passwordHasher) 
        : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public Task<bool> ChangePasswordAsync(int id, string currentPassword, string newPassword)
        {
            var user = _userRepository.GetUserByIdAsync(id);

            if (user == null || user.Result == null)
                throw new KeyNotFoundException("User not found.");

            if (newPassword.Length < 6)
                throw new ArgumentException("New password must be at least 6 characters long.");

            if (!_passwordHasher.VerifyPassword(currentPassword, user.Result.PasswordHash, user.Result.PasswordSalt))
                throw new UnauthorizedAccessException("Current password is incorrect.");

            if (_passwordHasher.VerifyPassword(newPassword, user.Result.PasswordHash, user.Result.PasswordSalt))
                throw new ArgumentException("New password must be different from the current password.");

            var (hashedPassword, salt) = _passwordHasher.HashPassword(newPassword);
            return _userRepository.ChangePasswordAsync(id, hashedPassword, salt);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetUserByUserameAsync(username);
        }

        public async Task<User> RegisterUserAsync(string username, string email, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                throw new ArgumentException("Username, email, and password must be provided.");
            

            var existingUser = await _userRepository.GetUserByUserameAsync(username);
            if (existingUser != null)
                throw new InvalidOperationException("Username already exists.");

            var (hashedPassword, salt) = _passwordHasher.HashPassword(password);

            var newUser = new User
            {
                Username = username,
                Email = email,
                PasswordHash = hashedPassword,
                PasswordSalt = salt
            };

            return await _userRepository.AddUserAsync(newUser);
        }

        public async Task<User> UpdateUserAsync(int id, string? username, string? email)
        {
            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(email))
                throw new ArgumentException("At least one of username or email must be provided for update.");

            return await _userRepository.UpdateUserAsync(id, username, email) 
                ?? throw new KeyNotFoundException("User not found.");
        }

        public async Task<bool> VerifyLoginAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUserameAsync(username);
            if (user == null)
                return false;

            return _passwordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
        }
    }
}
