namespace AgileBoard.Domain.Entities
{
    public record Sprint
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }

        // FKs
        public int ProjectId { get; init; }

        // Relations
        public virtual Project? Project { get; init; }
        public virtual ICollection<WorkItem>? WorkItems { get; init; }
    }
}
