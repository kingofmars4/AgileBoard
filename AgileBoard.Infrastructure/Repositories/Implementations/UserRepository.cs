using AgileBoard.Domain.Entities;
using AgileBoard.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgileBoard.Infrastructure.Repositories.Implementations
{
#pragma warning disable CS8603
    public class UserRepository(AgileBoardDbContext context) 
        : IUserRepository
    {
        private readonly AgileBoardDbContext _context = context;

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        public async Task<User?> GetUserByUserameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> AddUserAsync(User newUser)
        {
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }

        public async Task<User> UpdateUserAsync(int id, string? username, string? email)
        {
            if (!string.IsNullOrEmpty(username))
                await _context.Users.Where(u => u.Id == id)
                    .ExecuteUpdateAsync(u => u
                        .SetProperty(u => u.Username, username)
                    );
            if (!string.IsNullOrEmpty(email))
                await _context.Users.Where(u => u.Id == id)
                    .ExecuteUpdateAsync(u => u
                        .SetProperty(u => u.Email, email)
                    );

            return await GetUserByIdAsync(id);
        }

        public async Task<bool> ChangePasswordAsync(int id, string newPasswordHash, byte[] newPasswordSalt)
        {
            var result = await _context.Users.Where(u => u.Id == id)
                .ExecuteUpdateAsync(u => u
                    .SetProperty(u => u.PasswordHash, newPasswordHash)
                    .SetProperty(u => u.PasswordSalt, newPasswordSalt)
                );

            return result > 0;
        }
    }
}
