namespace AgileBoard.API.DTOs
{
    public record LoginResponseDTO (string Token, UserDTO User, string Message);
    public record AuthUserDTO (int Id, string Username, string Message);
}
