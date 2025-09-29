using static AgileBoard.Domain.Entities.WorkItem;

namespace AgileBoard.API.DTOs
{
    public record WorkItemDTO
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public WorkItemState State { get; init; }
        public int Index { get; init; }
        public int ProjectId { get; init; }
        public string ProjectName { get; init; } = string.Empty;
        public int? SprintId { get; init; }
        public string? SprintName { get; init; }
        public IEnumerable<UserDTO>? AssignedUsers { get; init; }
        public IEnumerable<TagDTO>? Tags { get; init; }
    }

    public record WorkItemSummaryDTO
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public WorkItemState State { get; init; }
        public int Index { get; init; }
        public int ProjectId { get; init; }
        public int? SprintId { get; init; }
        public int AssignedUsersCount { get; init; }
        public int TagsCount { get; init; }
    }

    public record CreateWorkItemDTO(string Name, string? Description, int ProjectId, WorkItemState State = WorkItemState.ToDo, int? SprintId = null);
    
    public record UpdateWorkItemDTO(string? Name, string? Description, WorkItemState? State, int? Index, int? SprintId);

    public record AssignUserDTO(int UserId);
    public record UnassignUserDTO(int UserId);
    public record AddTagToWorkItemDTO(int TagId);
    public record RemoveTagFromWorkItemDTO(int TagId);
    public record MoveToSprintDTO(int? SprintId);
    public record UpdateIndexDTO(int NewIndex);
}