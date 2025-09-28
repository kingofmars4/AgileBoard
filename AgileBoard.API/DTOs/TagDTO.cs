namespace AgileBoard.API.DTOs
{
    public record TagDTO(int Id, string Name);
    public record CreateTagDTO(string Name);
    public record UpdateTagDTO(string NewName);
}