namespace AgileBoard.Domain.Entities
{
    public record WorkItem
    {
        public enum WorkItemState
        {
            ToDo,
            Doing,
            Done
        }

        public int Id { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
        public WorkItemState State { get; init; }
        public int Index { get; init; }

        // FKs
        public int ProjectId { get; init; }
        public int? SprintId { get; init; }

        // Relations
        public virtual Project? Project { get; init; }
        public virtual Sprint? Sprint { get; init; }
        public virtual ICollection<User>? AssignedUsers { get; init; }
        public virtual ICollection<Tag>? Tags { get; init; }
    }
}
