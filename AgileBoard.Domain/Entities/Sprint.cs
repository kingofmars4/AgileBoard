namespace AgileBoard.Domain.Entities
{
    public class Sprint
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // FKs
        public int ProjectId { get; set; }

        // Relations
        public virtual Project? Project { get; set; }
        public virtual ICollection<WorkItem>? WorkItems { get; set; }
    }
}
