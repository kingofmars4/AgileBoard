namespace AgileBoard.Domain.Entities
{
    public record Project
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
        public DateTime CreationDate { get; init; }

        // FKs
        public int OwnerId { get; init; }

        // Relations
        public virtual User? Owner{ get; init; }
        public virtual ICollection<User>? Participants { get; init; }
        public virtual ICollection<WorkItem>? WorkItems { get; init; }
        public virtual ICollection<Sprint>? Sprints { get; init; }
    }
}
