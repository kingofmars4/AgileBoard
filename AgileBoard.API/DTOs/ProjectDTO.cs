namespace AgileBoard.API.DTOs
{
    public record ProjectDTO(string Name, string Description, int OwnerId, DateTime CreationDate);
    public record CreateProjectDTO(string Name, string Description, int OwnerId);
}
