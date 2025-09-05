namespace AgileBoard.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        // Relations
        public virtual ICollection<Project>? OwnedProjects { get; set; }
        public virtual ICollection<Project>? ParticipatingProjects { get; set; }
        public virtual ICollection<WorkItem>? AssignedWorkItems { get; set; }
    }
}
