namespace AgileBoard.API.DTOs
{
    public record UserDTO(int ID, string Username);
    public record CreateUserDTO(string Username, string Email, string Password);
}
