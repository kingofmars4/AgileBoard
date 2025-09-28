using AgileBoard.Domain.Entities;
using AgileBoard.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgileBoard.Infrastructure.Repositories.Implementations
{
    public class TagRepository(AgileBoardDbContext context) 
        : ITagRepository
    {
        private readonly AgileBoardDbContext _context = context;

        public async Task<Tag?> GetTagByIdAsync(int id)
        {
            return await _context.Tags.FindAsync(id);
        }
        public async Task<Tag?> GetTagByNameAsync(string name)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.Name == name);
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            return await _context.Tags.ToListAsync();
        }

        public async Task<Tag> AddTagAsync(Tag newTag)
        {
            await _context.Tags.AddAsync(newTag);
            await _context.SaveChangesAsync();

            return newTag;
        }

        public async Task<Tag?> UpdateTagAsync(int id, string? name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                await _context.Tags.Where(t => t.Id == id)
                    .ExecuteUpdateAsync(t => t.SetProperty(t => t.Name, name));
            }

            return await GetTagByIdAsync(id);
        }

        public async Task<bool> DeleteTagAsync(int id)
        {
            return await _context.Tags
                .Where(t => t.Id == id)
                .ExecuteDeleteAsync()
                .ContinueWith(t => t.Result > 0);
        }
    }
}
