using AgileBoard.API.DTOs;
using AgileBoard.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace AgileBoard.Tests
{
    [TestFixture]
    public class UserAuthenticationTests
    {
        private TestWebApplicationFactory<API.Program> _factory;
        private HttpClient _client;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _factory = new TestWebApplicationFactory<API.Program>();
            _client = _factory.CreateClient();
        }

        [SetUp]
        public async Task SetUp()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AgileBoardDbContext>();
            
            await context.Database.EnsureCreatedAsync();
            
            if (context.Users.Any())
            {
                context.Users.RemoveRange(context.Users);
                await context.SaveChangesAsync();
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        [Test]
        public async Task VerifyPassword_WithValidCredentials()
        {
            var password = $"TestPassword{new Random().Next(10000, 99999)}!";
            var createUserDto = new CreateUserDTO("testuser", "testemail@email.com", password);
            
            var registerResponse = await _client.PostAsJsonAsync("/api/users/register", createUserDto);
            
            Assert.That(registerResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var loginDto = new LoginUserDTO(createUserDto.Username, password);
            var loginResponse = await _client.PostAsJsonAsync("/api/users/login", loginDto);
            
            Assert.That(loginResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var loginContent = await loginResponse.Content.ReadAsStringAsync();
            Assert.That(loginContent, Does.Contain("Login successful"));
            Assert.That(loginContent, Does.Contain("testuser"));
        }

        [Test]
        public async Task VerifyPassword_WithInvalidPassword()
        {
            var registerDto = new CreateUserDTO("testuser2", "test2@example.com", "CorrectPassword123!");

            var registerResponse = await _client.PostAsJsonAsync("/api/users/register", registerDto);
            Assert.That(registerResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var loginDto = new LoginUserDTO("testuser2", "WrongPassword123!");
            var loginResponse = await _client.PostAsJsonAsync("/api/users/login", loginDto);

            Assert.That(loginResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task VerifyPassword_WithNonExistentUser()
        {
            var loginDto = new LoginUserDTO("nonexistent", "SomePassword123!");
            var loginResponse = await _client.PostAsJsonAsync("/api/users/login", loginDto);

            Assert.That(loginResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task VerifyPassword_MultipleUsers()
        {
            var user1 = new CreateUserDTO("user1", "user1@example.com", "Password1!");
            var user2 = new CreateUserDTO("user2", "user2@example.com", "Password2!");

            await _client.PostAsJsonAsync("/api/users/register", user1);
            await _client.PostAsJsonAsync("/api/users/register", user2);

            var login1 = await _client.PostAsJsonAsync("/api/users/login",
                new LoginUserDTO("user1", "Password1!"));
            var login2 = await _client.PostAsJsonAsync("/api/users/login",
                new LoginUserDTO("user2", "Password2!"));

            Assert.Multiple(() =>
            {
                Assert.That(login1.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(login2.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            });

            var crossLogin1 = await _client.PostAsJsonAsync("/api/users/login",
                new LoginUserDTO("user1", "Password2!"));
            var crossLogin2 = await _client.PostAsJsonAsync("/api/users/login",
                new LoginUserDTO("user2", "Password1!"));

            Assert.Multiple(() =>
            {
                Assert.That(crossLogin1.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
                Assert.That(crossLogin2.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            });
        }

        [Test]
        public async Task VerifyPassword_WithSpecialCharacters()
        {
            var complexPassword = "Pássw@rd123!#$%";
            var registerDto = new CreateUserDTO("specialuser", "special@example.com", complexPassword);

            await _client.PostAsJsonAsync("/api/users/register", registerDto);
            var loginResponse = await _client.PostAsJsonAsync("/api/users/login",
                new LoginUserDTO("specialuser", complexPassword));

            Assert.That(loginResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}
