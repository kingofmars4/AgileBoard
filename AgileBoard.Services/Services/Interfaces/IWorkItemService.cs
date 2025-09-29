using AgileBoard.Domain.Common;
using AgileBoard.Domain.Entities;

namespace AgileBoard.Services.Services.Interfaces
{
    public interface IWorkItemService
    {
        Task<Result<IEnumerable<WorkItem>>> GetAllWorkItemsAsync();
        Task<Result<WorkItem>> GetWorkItemByIdAsync(int id);
        Task<Result<IEnumerable<WorkItem>>> GetWorkItemsByProjectIdAsync(int projectId);
        Task<Result<IEnumerable<WorkItem>>> GetWorkItemsBySprintIdAsync(int sprintId);
        Task<Result<IEnumerable<WorkItem>>> GetWorkItemsByStateAsync(WorkItem.WorkItemState state);
        Task<Result<IEnumerable<WorkItem>>> GetWorkItemsByAssignedUserIdAsync(int userId);
        Task<Result<WorkItem>> CreateWorkItemAsync(string name, string? description, int projectId, WorkItem.WorkItemState state = WorkItem.WorkItemState.ToDo, int? sprintId = null);
        Task<Result<WorkItem>> UpdateWorkItemAsync(int id, string? name, string? description, WorkItem.WorkItemState? state, int? index, int? sprintId);
        Task<Result<bool>> DeleteWorkItemAsync(int id);
        Task<Result<bool>> AssignUserAsync(int workItemId, int userId);
        Task<Result<bool>> UnassignUserAsync(int workItemId, int userId);
        Task<Result<bool>> AddTagAsync(int workItemId, int tagId);
        Task<Result<bool>> RemoveTagAsync(int workItemId, int tagId);
        Task<Result<bool>> MoveToSprintAsync(int workItemId, int? sprintId);
        Task<Result<bool>> UpdateIndexAsync(int workItemId, int newIndex);
    }
}