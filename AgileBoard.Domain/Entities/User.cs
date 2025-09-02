namespace AgileBoard.Domain.Entities
{
    public record User
    {
        public int Id { get; init; }
        public string Name { get; init; }

        // Relations
        public virtual ICollection<Project> OwnedProjects { get; init; }
        public virtual ICollection<Project> ParticipatingProjects { get; init; }
        public virtual ICollection<Task> AssignedTasks { get; init; }
    }
}
