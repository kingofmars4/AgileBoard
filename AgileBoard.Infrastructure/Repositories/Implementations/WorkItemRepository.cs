using AgileBoard.Domain.Entities;
using AgileBoard.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgileBoard.Infrastructure.Repositories.Implementations
{
    public class WorkItemRepository(AgileBoardDbContext context) : IWorkItemRepository
    {
        private readonly AgileBoardDbContext _context = context;

        public async Task<WorkItem> AddWorkItemAsync(WorkItem workItem)
        {
            await _context.WorkItems.AddAsync(workItem);
            await _context.SaveChangesAsync();
            
            return await GetWorkItemByIdAsync(workItem.Id) ?? workItem;
        }

        public async Task<WorkItem?> GetWorkItemByIdAsync(int id)
        {
            return await _context.WorkItems
                .Include(w => w.Project)
                .ThenInclude(p => p!.Owner)
                .Include(w => w.Sprint)
                .Include(w => w.AssignedUsers)
                .Include(w => w.Tags)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<IEnumerable<WorkItem>> GetAllWorkItemsAsync()
        {
            return await _context.WorkItems
                .Include(w => w.Project)
                .ThenInclude(p => p!.Owner)
                .Include(w => w.Sprint)
                .Include(w => w.AssignedUsers)
                .Include(w => w.Tags)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkItem>> GetWorkItemsByProjectIdAsync(int projectId)
        {
            return await _context.WorkItems
                .Include(w => w.Project)
                .ThenInclude(p => p!.Owner)
                .Include(w => w.Sprint)
                .Include(w => w.AssignedUsers)
                .Include(w => w.Tags)
                .Where(w => w.ProjectId == projectId)
                .OrderBy(w => w.Index)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkItem>> GetWorkItemsBySprintIdAsync(int sprintId)
        {
            return await _context.WorkItems
                .Include(w => w.Project)
                .ThenInclude(p => p!.Owner)
                .Include(w => w.Sprint)
                .Include(w => w.AssignedUsers)
                .Include(w => w.Tags)
                .Where(w => w.SprintId == sprintId)
                .OrderBy(w => w.Index)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkItem>> GetWorkItemsByStateAsync(WorkItem.WorkItemState state)
        {
            return await _context.WorkItems
                .Include(w => w.Project)
                .ThenInclude(p => p!.Owner)
                .Include(w => w.Sprint)
                .Include(w => w.AssignedUsers)
                .Include(w => w.Tags)
                .Where(w => w.State == state)
                .OrderBy(w => w.Index)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkItem>> GetWorkItemsByAssignedUserIdAsync(int userId)
        {
            return await _context.WorkItems
                .Include(w => w.Project)
                .ThenInclude(p => p!.Owner)
                .Include(w => w.Sprint)
                .Include(w => w.AssignedUsers)
                .Include(w => w.Tags)
                .Where(w => w.AssignedUsers!.Any(u => u.Id == userId))
                .OrderBy(w => w.Index)
                .ToListAsync();
        }

        public async Task<WorkItem?> UpdateWorkItemAsync(int id, string? name, string? description, WorkItem.WorkItemState? state, int? index, int? sprintId)
        {
            var workItem = await _context.WorkItems.FindAsync(id);
            if (workItem == null) return null;

            if (!string.IsNullOrEmpty(name))
                workItem.Name = name;

            if (!string.IsNullOrEmpty(description))
                workItem.Description = description;

            if (state.HasValue)
                workItem.State = state.Value;

            if (index.HasValue)
                workItem.Index = index.Value;

            if (sprintId.HasValue)
                workItem.SprintId = sprintId.Value;

            await _context.SaveChangesAsync();
            return await GetWorkItemByIdAsync(id);
        }

        public async Task<bool> DeleteWorkItemAsync(int id)
        {
            var result = await _context.WorkItems
                .Where(w => w.Id == id)
                .ExecuteDeleteAsync();
            
            return result > 0;
        }

        public async Task<bool> AssignUserToWorkItemAsync(int workItemId, int userId)
        {
            var workItem = await _context.WorkItems
                .Include(w => w.AssignedUsers)
                .FirstOrDefaultAsync(w => w.Id == workItemId);
            
            var user = await _context.Users.FindAsync(userId);
            
            if (workItem == null || user == null) return false;
            
            if (workItem.AssignedUsers?.Any(u => u.Id == userId) == true) return false;
            
            workItem.AssignedUsers ??= [];
            workItem.AssignedUsers.Add(user);
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnassignUserFromWorkItemAsync(int workItemId, int userId)
        {
            var workItem = await _context.WorkItems
                .Include(w => w.AssignedUsers)
                .FirstOrDefaultAsync(w => w.Id == workItemId);
            
            if (workItem?.AssignedUsers == null) return false;
            
            var user = workItem.AssignedUsers.FirstOrDefault(u => u.Id == userId);
            if (user == null) return false;
            
            workItem.AssignedUsers.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddTagToWorkItemAsync(int workItemId, int tagId)
        {
            var workItem = await _context.WorkItems
                .Include(w => w.Tags)
                .FirstOrDefaultAsync(w => w.Id == workItemId);
            
            var tag = await _context.Tags.FindAsync(tagId);
            
            if (workItem == null || tag == null) return false;
            
            if (workItem.Tags?.Any(t => t.Id == tagId) == true) return false;
            
            workItem.Tags ??= [];
            workItem.Tags.Add(tag);
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveTagFromWorkItemAsync(int workItemId, int tagId)
        {
            var workItem = await _context.WorkItems
                .Include(w => w.Tags)
                .FirstOrDefaultAsync(w => w.Id == workItemId);
            
            if (workItem?.Tags == null) return false;
            
            var tag = workItem.Tags.FirstOrDefault(t => t.Id == tagId);
            if (tag == null) return false;
            
            workItem.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MoveWorkItemToSprintAsync(int workItemId, int? sprintId)
        {
            var workItem = await _context.WorkItems.FindAsync(workItemId);
            if (workItem == null) return false;

            workItem.SprintId = sprintId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateWorkItemIndexAsync(int workItemId, int newIndex)
        {
            var workItem = await _context.WorkItems.FindAsync(workItemId);
            if (workItem == null) return false;

            workItem.Index = newIndex;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}