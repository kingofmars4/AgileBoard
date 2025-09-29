using AgileBoard.Domain.Common;
using AgileBoard.Domain.Entities;

namespace AgileBoard.Services.Services.Interfaces
{
    public interface ISprintService
    {
        Task<Result<IEnumerable<Sprint>>> GetAllSprintsAsync();
        Task<Result<Sprint>> GetSprintByIdAsync(int id);
        Task<Result<Sprint>> GetSprintByNameAsync(string name, int projectId);
        Task<Result<IEnumerable<Sprint>>> GetSprintsByProjectIdAsync(int projectId);
        Task<Result<IEnumerable<Sprint>>> GetActiveSprintsAsync();
        Task<Result<IEnumerable<Sprint>>> GetSprintsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Result<Sprint>> CreateSprintAsync(string name, string? description, int projectId, DateTime startDate, DateTime endDate);
        Task<Result<Sprint>> UpdateSprintAsync(int id, string? name, string? description, DateTime? startDate, DateTime? endDate);
        Task<Result<bool>> DeleteSprintAsync(int id);
    }
}