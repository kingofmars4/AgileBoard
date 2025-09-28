using AgileBoard.Domain.Entities;
using AgileBoard.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgileBoard.Infrastructure.Repositories.Implementations
{
    public class ProjectRepository(AgileBoardDbContext context) : IProjectRepository
    {
        private readonly AgileBoardDbContext _context = context;

        public async Task<Project> AddProjectAsync(Project project)
        {
            project.CreationDate = DateTime.UtcNow;
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.Participants)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Project?> GetProjectByNameAsync(string name)
        {
            return await _context.Projects
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Name == name);
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.Participants)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetProjectsByOwnerIdAsync(int ownerId)
        {
            return await _context.Projects
                .Include(p => p.Owner)
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetProjectsByParticipantIdAsync(int participantId)
        {
            return await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.Participants)
                .Where(p => p.Participants!.Any(u => u.Id == participantId))
                .ToListAsync();
        }

        public async Task<Project?> UpdateProjectAsync(int id, string? name, string? description)
        {
            var updateQuery = _context.Projects.Where(p => p.Id == id);

            if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(description))
            {
                await updateQuery.ExecuteUpdateAsync(p => p
                    .SetProperty(p => p.Name, name)
                    .SetProperty(p => p.Description, description));
            }

            return await GetProjectByIdAsync(id);
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            var result = await _context.Projects
                .Where(p => p.Id == id)
                .ExecuteDeleteAsync();
            
            return result > 0;
        }

        public async Task<bool> AddParticipantToProjectAsync(int projectId, int userId)
        {
            var project = await _context.Projects
                .Include(p => p.Participants)
                .FirstOrDefaultAsync(p => p.Id == projectId);
            
            var user = await _context.Users.FindAsync(userId);
            
            if (project == null || user == null) return false;
            
            if (project.Participants?.Any(u => u.Id == userId) == true) return false; // Already participant
            
            project.Participants ??= new List<User>();
            project.Participants.Add(user);
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveParticipantFromProjectAsync(int projectId, int userId)
        {
            var project = await _context.Projects
                .Include(p => p.Participants)
                .FirstOrDefaultAsync(p => p.Id == projectId);
            
            if (project?.Participants == null) return false;
            
            var participant = project.Participants.FirstOrDefault(u => u.Id == userId);
            if (participant == null) return false;
            
            project.Participants.Remove(participant);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
