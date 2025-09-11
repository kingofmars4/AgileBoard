namespace AgileBoard.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public Byte[] PasswordSalt { get; set; } = [];


        // Relations
        public virtual ICollection<Project>? OwnedProjects { get; set; }
        public virtual ICollection<Project>? ParticipatingProjects { get; set; }
        public virtual ICollection<WorkItem>? AssignedWorkItems { get; set; }

        
    }
}
