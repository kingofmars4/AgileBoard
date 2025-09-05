namespace AgileBoard.Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreationDate { get; set; }

        // FKs
        public int OwnerId { get; set; }

        // Relations
        public virtual User? Owner{ get; set; }
        public virtual ICollection<User>? Participants { get; set; }
        public virtual ICollection<WorkItem>? WorkItems { get; set; }
        public virtual ICollection<Sprint>? Sprints { get; set; }
    }
}
