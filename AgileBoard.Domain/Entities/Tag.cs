namespace AgileBoard.Domain.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        // Relations
        public virtual ICollection<WorkItem>? WorkItems { get; set; }
    }
}
