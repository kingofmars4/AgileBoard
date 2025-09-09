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

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            return user ?? throw new KeyNotFoundException("User not found.");
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

        public async Task<bool> VerifiyLoginAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUserameAsync(username);
            if (user == null)
                return false;

            return _passwordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
        }
    }
}
