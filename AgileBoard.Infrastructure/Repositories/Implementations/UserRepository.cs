using AgileBoard.Domain.Entities;
using AgileBoard.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgileBoard.Infrastructure.Repositories.Implementations
{
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

    }
}
