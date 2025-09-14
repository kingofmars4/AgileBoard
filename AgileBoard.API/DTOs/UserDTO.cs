namespace AgileBoard.API.DTOs
{
    public record UserDTO(int Id, string Username, string Email);
    public record CreateUserDTO(string Username, string Email, string Password);
    public record LoginUserDTO(string Username, string Password);
    public record UpdateUserDTO(string? Username, string? Email);
    public record ChangePasswordDTO(string CurrentPassword, string NewPassword);
}
