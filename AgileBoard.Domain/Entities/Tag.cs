namespace AgileBoard.Domain.Entities
{
    public record Tag
    {
        public int Id { get; init; }
        public string? Name { get; init; }

        // Relations
        public virtual ICollection<WorkItem>? WorkItems { get; init; }
    }
}
