namespace AgileBoard.Domain.Entities
{
    public class WorkItem
    {
        public enum WorkItemState
        {
            ToDo,
            Doing,
            Done
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public WorkItemState State { get; set; }
        public int Index { get; set; }

        // FKs
        public int ProjectId { get; set; }
        public int? SprintId { get; set; }

        // Relations
        public virtual Project? Project { get; set; }
        public virtual Sprint? Sprint { get; set; }
        public virtual ICollection<User>? AssignedUsers { get; set; }
        public virtual ICollection<Tag>? Tags { get; set; }
    }
}
