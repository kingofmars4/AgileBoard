namespace AgileBoard.API.DTOs
{
    public record ProjectDTO(
        int Id, 
        string Name, 
        string Description, 
        int OwnerId, 
        string OwnerUsername,
        DateTime CreationDate,
        IEnumerable<UserDTO>? Participants
    );

    public record ProjectSummaryDTO(
        int Id,
        string Name,
        string Description,
        string OwnerUsername,
        DateTime CreationDate,
        int ParticipantCount
    );

    public record CreateProjectDTO(string Name, string Description);
    public record UpdateProjectDTO(string? Name, string? Description);
    
    public record AddParticipantDTO(int UserId);
    public record RemoveParticipantDTO(int UserId);
}
