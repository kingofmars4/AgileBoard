using AgileBoard.Domain.Entities;
using AgileBoard.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgileBoard.Infrastructure.Repositories.Implementations
{
    public class SprintRepository(AgileBoardDbContext context) : ISprintRepository
    {
        private readonly AgileBoardDbContext _context = context;

        public async Task<Sprint> AddSprintAsync(Sprint sprint)
        {
            await _context.Sprints.AddAsync(sprint);
            await _context.SaveChangesAsync();
            
            return await GetSprintByIdAsync(sprint.Id) ?? sprint;
        }

        public async Task<Sprint?> GetSprintByIdAsync(int id)
        {
            return await _context.Sprints
                .Include(s => s.Project)
                .ThenInclude(p => p!.Owner)
                .Include(s => s.WorkItems)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Sprint?> GetSprintByNameAsync(string name, int projectId)
        {
            return await _context.Sprints
                .Include(s => s.Project)
                .ThenInclude(p => p!.Owner)
                .Include(s => s.WorkItems)
                .FirstOrDefaultAsync(s => s.Name == name && s.ProjectId == projectId);
        }

        public async Task<IEnumerable<Sprint>> GetAllSprintsAsync()
        {
            return await _context.Sprints
                .Include(s => s.Project)
                .ThenInclude(p => p!.Owner)
                .Include(s => s.WorkItems)
                .OrderBy(s => s.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sprint>> GetSprintsByProjectIdAsync(int projectId)
        {
            return await _context.Sprints
                .Include(s => s.Project)
                .ThenInclude(p => p!.Owner)
                .Include(s => s.WorkItems)
                .Where(s => s.ProjectId == projectId)
                .OrderBy(s => s.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sprint>> GetActiveSprintsAsync()
        {
            var currentDate = DateTime.UtcNow;
            return await _context.Sprints
                .Include(s => s.Project)
                .ThenInclude(p => p!.Owner)
                .Include(s => s.WorkItems)
                .Where(s => s.StartDate <= currentDate && s.EndDate >= currentDate)
                .OrderBy(s => s.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sprint>> GetSprintsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Sprints
                .Include(s => s.Project)
                .ThenInclude(p => p!.Owner)
                .Include(s => s.WorkItems)
                .Where(s => s.StartDate <= endDate && s.EndDate >= startDate)
                .OrderBy(s => s.StartDate)
                .ToListAsync();
        }

        public async Task<Sprint?> UpdateSprintAsync(int id, string? name, string? description, DateTime? startDate, DateTime? endDate)
        {
            var sprint = await _context.Sprints.FindAsync(id);
            if (sprint == null) return null;

            if (!string.IsNullOrEmpty(name))
                sprint.Name = name;

            if (!string.IsNullOrEmpty(description))
                sprint.Description = description;

            if (startDate.HasValue)
                sprint.StartDate = startDate.Value;

            if (endDate.HasValue)
                sprint.EndDate = endDate.Value;

            await _context.SaveChangesAsync();
            return await GetSprintByIdAsync(id);
        }

        public async Task<bool> DeleteSprintAsync(int id)
        {
            var result = await _context.Sprints
                .Where(s => s.Id == id)
                .ExecuteDeleteAsync();
            
            return result > 0;
        }

        public async Task<bool> HasWorkItemsAsync(int sprintId)
        {
            return await _context.WorkItems
                .AnyAsync(w => w.SprintId == sprintId);
        }
    }
}