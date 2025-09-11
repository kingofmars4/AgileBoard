using AgileBoard.Domain.Common;
using AgileBoard.Domain.Constants;
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

        public async Task<Result> VerifyLoginAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUserameAsync(username);
            if (user == null)
                return Result.Unauthorized(Messages.Authentication.InvalidCredentials);

            var isValid = _passwordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
            if (!isValid)
                return Result.Unauthorized(Messages.Authentication.InvalidCredentials);

            return Result.Success();
        }

        public async Task<Result<User>> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                return Result<User>.NotFound(Messages.UserRetrieval.UserNotFound);

            return Result<User>.Success(user);
        }

        public async Task<Result<User>> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetUserByUserameAsync(username);
            if (user == null)
                return Result<User>.NotFound(Messages.UserRetrieval.UserNotFound);

            return Result<User>.Success(user);
        }

        public async Task<Result<IEnumerable<User>>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            if (!users.Any())
                return Result<IEnumerable<User>>.NotFound(Messages.UserRetrieval.UsersNotFound);

            return Result<IEnumerable<User>>.Success(users);
        }

        public async Task<Result<User>> RegisterUserAsync(string username, string email, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return Result<User>.BadRequest(Messages.Registration.UsernamePasswordEmailRequired);

            var existingUser = await _userRepository.GetUserByUserameAsync(username);
            if (existingUser != null)
                return Result<User>.Conflict(Messages.Registration.UsernameAlreadyExists);

            try
            {
                var (hashedPassword, salt) = _passwordHasher.HashPassword(password);

                var newUser = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = hashedPassword,
                    PasswordSalt = salt
                };

                var createdUser = await _userRepository.AddUserAsync(newUser);
                return Result<User>.Success(createdUser);
            }
            catch (Exception)
            {
                return Result<User>.Failure("Database error occurred");
            }
        }

        public async Task<Result<User>> UpdateUserAsync(int id, string? username, string? email)
        {
            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(email))
                return Result<User>.BadRequest(Messages.UserUpdate.AtLeastOneFieldRequired);

            try
            {
                var updatedUser = await _userRepository.UpdateUserAsync(id, username, email);
                if (updatedUser == null)
                    return Result<User>.NotFound(Messages.UserRetrieval.UserNotFound);

                return Result<User>.Success(updatedUser);
            }
            catch (Exception)
            {
                return Result<User>.Failure("Database error occurred");
            }
        }

        public async Task<Result> ChangePasswordAsync(int id, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                return Result.NotFound(Messages.UserRetrieval.UserNotFound);

            if (newPassword.Length < 6)
                return Result.BadRequest(Messages.PasswordChange.PasswordMinimumLength);

            if (!_passwordHasher.VerifyPassword(currentPassword, user.PasswordHash, user.PasswordSalt))
                return Result.Unauthorized(Messages.PasswordChange.CurrentPasswordIncorrect);

            if (_passwordHasher.VerifyPassword(newPassword, user.PasswordHash, user.PasswordSalt))
                return Result.BadRequest(Messages.PasswordChange.NewPasswordMustBeDifferent);

            try
            {
                var (hashedPassword, salt) = _passwordHasher.HashPassword(newPassword);
                await _userRepository.ChangePasswordAsync(id, hashedPassword, salt);
                return Result.Success();
            }
            catch (Exception)
            {
                return Result.Failure("Database error occurred");
            }
        }
    }
}
