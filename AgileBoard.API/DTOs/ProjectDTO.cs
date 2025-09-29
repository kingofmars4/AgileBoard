namespace AgileBoard.API.DTOs
{
    public record ProjectDTO
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public int OwnerId { get; init; }
        public string OwnerUsername { get; init; } = "Unknown";
        public DateTime CreationDate { get; init; }
        public IEnumerable<UserDTO>? Participants { get; init; }
    }

    public record ProjectSummaryDTO
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string OwnerUsername { get; init; } = "Unknown";
        public DateTime CreationDate { get; init; }
        public int ParticipantCount { get; init; }
    }

    public record CreateProjectDTO(string Name, string Description);
    public record UpdateProjectDTO(string? Name, string? Description);
    
    public record AddParticipantDTO(int UserId);
    public record RemoveParticipantDTO(int UserId);
}
