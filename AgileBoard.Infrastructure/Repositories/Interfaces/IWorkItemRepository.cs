using AgileBoard.Domain.Entities;

namespace AgileBoard.Infrastructure.Repositories.Interfaces
{
    public interface IWorkItemRepository
    {
        Task<WorkItem> AddWorkItemAsync(WorkItem workItem);
        Task<WorkItem?> GetWorkItemByIdAsync(int id);
        Task<IEnumerable<WorkItem>> GetAllWorkItemsAsync();
        Task<IEnumerable<WorkItem>> GetWorkItemsByProjectIdAsync(int projectId);
        Task<IEnumerable<WorkItem>> GetWorkItemsBySprintIdAsync(int sprintId);
        Task<IEnumerable<WorkItem>> GetWorkItemsByStateAsync(WorkItem.WorkItemState state);
        Task<IEnumerable<WorkItem>> GetWorkItemsByAssignedUserIdAsync(int userId);
        Task<WorkItem?> UpdateWorkItemAsync(int id, string? name, string? description, WorkItem.WorkItemState? state, int? index, int? sprintId);
        Task<bool> DeleteWorkItemAsync(int id);
        Task<bool> AssignUserToWorkItemAsync(int workItemId, int userId);
        Task<bool> UnassignUserFromWorkItemAsync(int workItemId, int userId);
        Task<bool> AddTagToWorkItemAsync(int workItemId, int tagId);
        Task<bool> RemoveTagFromWorkItemAsync(int workItemId, int tagId);
        Task<bool> MoveWorkItemToSprintAsync(int workItemId, int? sprintId);
        Task<bool> UpdateWorkItemIndexAsync(int workItemId, int newIndex);
    }
}