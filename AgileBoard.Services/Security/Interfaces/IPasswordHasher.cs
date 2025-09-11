namespace AgileBoard.Services.Security.Interfaces
{
    public interface IPasswordHasher
    {
        (string HashedPassword , byte[] Salt) HashPassword(string password);
        bool VerifyPassword(string enteredPassword, string storedHash, byte[] salt);
    }
}
