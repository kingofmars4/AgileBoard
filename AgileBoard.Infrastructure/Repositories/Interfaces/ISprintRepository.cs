using AgileBoard.Domain.Entities;

namespace AgileBoard.Infrastructure.Repositories.Interfaces
{
    public interface ISprintRepository
    {
        Task<Sprint> AddSprintAsync(Sprint sprint);
        Task<Sprint?> GetSprintByIdAsync(int id);
        Task<Sprint?> GetSprintByNameAsync(string name, int projectId);
        Task<IEnumerable<Sprint>> GetAllSprintsAsync();
        Task<IEnumerable<Sprint>> GetSprintsByProjectIdAsync(int projectId);
        Task<IEnumerable<Sprint>> GetActiveSprintsAsync();
        Task<IEnumerable<Sprint>> GetSprintsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Sprint?> UpdateSprintAsync(int id, string? name, string? description, DateTime? startDate, DateTime? endDate);
        Task<bool> DeleteSprintAsync(int id);
        Task<bool> HasWorkItemsAsync(int sprintId);
    }
}