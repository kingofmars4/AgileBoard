using AgileBoard.Services.Security.Interfaces;
using System.Security.Cryptography;

namespace AgileBoard.Services.Security.Implementations
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16; // 128 bit
        private const int KeySize = 32; // 256 bit
        private const int Iterations = 10000; // Number of iterations for PBKDF2

        public (string HashedPassword, byte[] Salt) HashPassword(string password)
        {
            if (String.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));

            var salt = new byte[SaltSize];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            var pbkdf2 = new Rfc2898DeriveBytes(
                password: password,
                salt: salt,
                iterations: Iterations,
                hashAlgorithm: HashAlgorithmName.SHA256
            );

            byte[] hash = pbkdf2.GetBytes(KeySize);
            string hashedPassword = Convert.ToBase64String(hash);

            return (hashedPassword, salt);
        }

        public bool VerifyPassword(string enteredPassword, string storedHash, byte[] salt)
        {
            if (String.IsNullOrWhiteSpace(enteredPassword))
                throw new ArgumentException("Entered password cannot be null or empty.", nameof(enteredPassword));

            var pbkdf2 = new Rfc2898DeriveBytes(
                password: enteredPassword,
                salt: salt,
                iterations: Iterations,
                hashAlgorithm: HashAlgorithmName.SHA256
            );

            byte[] enteredHash = pbkdf2.GetBytes(KeySize);
            byte[] storedHashBytes = Convert.FromBase64String(storedHash);

            return enteredHash.SequenceEqual(storedHashBytes);
        }
    }
}
