using AgileBoard.Domain.Entities;
using AgileBoard.Infrastructure.Repositories.Interfaces;

namespace AgileBoard.Infrastructure.Repositories.Implementations
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AgileBoardDbContext _context;

        public ProjectRepository(AgileBoardDbContext context)
        {
            _context = context;
        }

        public async Task<Project> GetProjectByIdAsync(int id)
        {
            return await _context.Projects.FindAsync(id);
        }

        public async Task AddProjectAsync(Project project)
        {
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
        }
    }
}
