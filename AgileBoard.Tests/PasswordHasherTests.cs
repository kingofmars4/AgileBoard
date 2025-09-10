using AgileBoard.Services.Security.Implementations;

namespace AgileBoard.Tests
{
    public class PasswordHasherTests
    {
        private PasswordHasher _passwordHasher;

        [SetUp]
        public void SetUp()
        {
            _passwordHasher = new PasswordHasher();
        }

        [Test]
        public void VerifyPassword_WithCorrectPassword()
        {
            var password ="SecurePassword123!";
            var (hashedPassword, salt) = _passwordHasher.HashPassword(password);
            var result = _passwordHasher.VerifyPassword(password, hashedPassword, salt);

            Assert.That(result, Is.True);
        }

        [Test]
        public void VerifyPassword_WithIncorrectPassword()
        {
            var password = "SecurePassword123!";
            var (hashedPassword, salt) = _passwordHasher.HashPassword(password);
            var result = _passwordHasher.VerifyPassword("WrongPassword!", hashedPassword, salt);

            Assert.That(result, Is.False);
        }

        [Test]
        public void HashPassword_SamePlainText()
        {
            var password = "SamePassword123!";

            var (hash1, salt1) = _passwordHasher.HashPassword(password);
            var (hash2, salt2) = _passwordHasher.HashPassword(password);

            Assert.Multiple(() =>
            {
                Assert.That(hash1, Is.Not.EqualTo(hash2));
                Assert.That(salt1, Is.Not.EqualTo(salt2));
            });
        }
    }
}
