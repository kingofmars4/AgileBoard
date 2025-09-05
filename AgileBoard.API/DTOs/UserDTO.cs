namespace AgileBoard.API.DTOs
{
    public record UserDTO(int ID, string Name);
    public record CreateUserDTO(string Name);
}
