using AgileBoard.Domain.Common;
using AgileBoard.Domain.Constants;
using AgileBoard.Domain.Entities;
using AgileBoard.Infrastructure.Repositories.Interfaces;
using AgileBoard.Services.Services.Interfaces;

namespace AgileBoard.Services.Services.Implementations
{
    public class WorkItemService(IWorkItemRepository workItemRepository) : IWorkItemService
    {
        private readonly IWorkItemRepository _workItemRepository = workItemRepository;

        public async Task<Result<IEnumerable<WorkItem>>> GetAllWorkItemsAsync()
        {
            var workItems = await _workItemRepository.GetAllWorkItemsAsync();
            if (!workItems.Any())
                return Result<IEnumerable<WorkItem>>.NotFound(Messages.EntityNames.WorkItems, isPlural: true);

            return Result<IEnumerable<WorkItem>>.Success(workItems);
        }

        public async Task<Result<WorkItem>> GetWorkItemByIdAsync(int id)
        {
            var workItem = await _workItemRepository.GetWorkItemByIdAsync(id);
            if (workItem == null)
                return Result<WorkItem>.NotFound(Messages.EntityNames.WorkItem);

            return Result<WorkItem>.Success(workItem);
        }

        public async Task<Result<IEnumerable<WorkItem>>> GetWorkItemsByProjectIdAsync(int projectId)
        {
            var workItems = await _workItemRepository.GetWorkItemsByProjectIdAsync(projectId);
            if (!workItems.Any())
                return Result<IEnumerable<WorkItem>>.NotFound(Messages.WorkItems.NoWorkItemsFoundForProject);

            return Result<IEnumerable<WorkItem>>.Success(workItems);
        }

        public async Task<Result<IEnumerable<WorkItem>>> GetWorkItemsBySprintIdAsync(int sprintId)
        {
            var workItems = await _workItemRepository.GetWorkItemsBySprintIdAsync(sprintId);
            return Result<IEnumerable<WorkItem>>.Success(workItems);
        }

        public async Task<Result<IEnumerable<WorkItem>>> GetWorkItemsByStateAsync(WorkItem.WorkItemState state)
        {
            var workItems = await _workItemRepository.GetWorkItemsByStateAsync(state);
            return Result<IEnumerable<WorkItem>>.Success(workItems);
        }

        public async Task<Result<IEnumerable<WorkItem>>> GetWorkItemsByAssignedUserIdAsync(int userId)
        {
            var workItems = await _workItemRepository.GetWorkItemsByAssignedUserIdAsync(userId);
            return Result<IEnumerable<WorkItem>>.Success(workItems);
        }

        public async Task<Result<WorkItem>> CreateWorkItemAsync(string name, string? description, int projectId, WorkItem.WorkItemState state = WorkItem.WorkItemState.ToDo, int? sprintId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<WorkItem>.BadRequest(Messages.WorkItems.WorkItemNameRequired);

            if (projectId <= 0)
                return Result<WorkItem>.BadRequest(Messages.WorkItems.ProjectIdRequired);

            try
            {
                var newWorkItem = new WorkItem
                {
                    Name = name,
                    Description = description,
                    ProjectId = projectId,
                    State = state,
                    SprintId = sprintId,
                    Index = 0
                };

                var createdWorkItem = await _workItemRepository.AddWorkItemAsync(newWorkItem);
                return Result<WorkItem>.Success(createdWorkItem);
            }
            catch (Exception)
            {
                return Result<WorkItem>.Failure(Messages.Generic.InternalServerError);
            }
        }

        public async Task<Result<WorkItem>> UpdateWorkItemAsync(int id, string? name, string? description, WorkItem.WorkItemState? state, int? index, int? sprintId)
        {
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(description) && !state.HasValue && !index.HasValue && !sprintId.HasValue)
                return Result<WorkItem>.BadRequest(Messages.WorkItemUpdate.NoFieldsSpecified);

            try
            {
                var updatedWorkItem = await _workItemRepository.UpdateWorkItemAsync(id, name, description, state, index, sprintId);
                if (updatedWorkItem == null)
                    return Result<WorkItem>.NotFound(Messages.EntityNames.WorkItem);

                return Result<WorkItem>.Success(updatedWorkItem);
            }
            catch (Exception)
            {
                return Result<WorkItem>.Failure(Messages.Generic.InternalServerError);
            }
        }

        public async Task<Result<bool>> DeleteWorkItemAsync(int id)
        {
            var workItem = await _workItemRepository.GetWorkItemByIdAsync(id);
            if (workItem == null)
                return Result<bool>.NotFound(Messages.EntityNames.WorkItem);

            try
            {
                var deleted = await _workItemRepository.DeleteWorkItemAsync(id);
                return Result<bool>.Success(deleted);
            }
            catch (Exception)
            {
                return Result<bool>.Failure(Messages.Generic.InternalServerError);
            }
        }

        public async Task<Result<bool>> AssignUserAsync(int workItemId, int userId)
        {
            try
            {
                var assigned = await _workItemRepository.AssignUserToWorkItemAsync(workItemId, userId);
                if (!assigned)
                    return Result<bool>.BadRequest(Messages.WorkItemAssignment.AssignmentFailed);

                return Result<bool>.Success(true);
            }
            catch (Exception)
            {
                return Result<bool>.Failure(Messages.Generic.InternalServerError);
            }
        }

        public async Task<Result<bool>> UnassignUserAsync(int workItemId, int userId)
        {
            try
            {
                var unassigned = await _workItemRepository.UnassignUserFromWorkItemAsync(workItemId, userId);
                if (!unassigned)
                    return Result<bool>.BadRequest(Messages.WorkItemAssignment.UnassignmentFailed);

                return Result<bool>.Success(true);
            }
            catch (Exception)
            {
                return Result<bool>.Failure(Messages.Generic.InternalServerError);
            }
        }

        public async Task<Result<bool>> AddTagAsync(int workItemId, int tagId)
        {
            try
            {
                var added = await _workItemRepository.AddTagToWorkItemAsync(workItemId, tagId);
                if (!added)
                    return Result<bool>.BadRequest(Messages.WorkItemTags.AddTagFailed);

                return Result<bool>.Success(true);
            }
            catch (Exception)
            {
                return Result<bool>.Failure(Messages.Generic.InternalServerError);
            }
        }

        public async Task<Result<bool>> RemoveTagAsync(int workItemId, int tagId)
        {
            try
            {
                var removed = await _workItemRepository.RemoveTagFromWorkItemAsync(workItemId, tagId);
                if (!removed)
                    return Result<bool>.BadRequest(Messages.WorkItemTags.RemoveTagFailed);

                return Result<bool>.Success(true);
            }
            catch (Exception)
            {
                return Result<bool>.Failure(Messages.Generic.InternalServerError);
            }
        }

        public async Task<Result<bool>> MoveToSprintAsync(int workItemId, int? sprintId)
        {
            try
            {
                var moved = await _workItemRepository.MoveWorkItemToSprintAsync(workItemId, sprintId);
                if (!moved)
                    return Result<bool>.NotFound(Messages.EntityNames.WorkItem);

                return Result<bool>.Success(true);
            }
            catch (Exception)
            {
                return Result<bool>.Failure(Messages.Generic.InternalServerError);
            }
        }

        public async Task<Result<bool>> UpdateIndexAsync(int workItemId, int newIndex)
        {
            if (newIndex < 0)
                return Result<bool>.BadRequest(Messages.WorkItems.InvalidIndex);

            try
            {
                var updated = await _workItemRepository.UpdateWorkItemIndexAsync(workItemId, newIndex);
                if (!updated)
                    return Result<bool>.NotFound(Messages.EntityNames.WorkItem);

                return Result<bool>.Success(true);
            }
            catch (Exception)
            {
                return Result<bool>.Failure(Messages.Generic.InternalServerError);
            }
        }
    }
}