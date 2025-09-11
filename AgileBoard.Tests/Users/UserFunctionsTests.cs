using AgileBoard.API.DTOs;
using AgileBoard.Infrastructure;
using System.Net;
using System.Text.Json;

namespace AgileBoard.Tests.Users
{
    [TestFixture]
    public class UserFunctionsTests
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
        public async Task UpdateUser_WithValidData_ShouldUpdateSuccessfully()
        {
            var createUserDto = new CreateUserDTO("originaluser", "original@email.com", "Password123!");
            var registerResponse = await _client.PostAsJsonAsync("/api/users/register", createUserDto);
            Assert.That(registerResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var createdUser = await GetUserFromResponse(registerResponse);
            var updateDto = new UpdateUserDTO("updateduser", "updated@email.com");

            var updateResponse = await _client.PutAsJsonAsync($"/api/users/{createdUser.ID}", updateDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var updatedUser = await GetUserFromResponse(updateResponse);
            Assert.Multiple(() =>
            {
                Assert.That(updatedUser.Username, Is.EqualTo("updateduser"));
                Assert.That(updatedUser.Email, Is.EqualTo("updated@email.com"));
                Assert.That(updatedUser.ID, Is.EqualTo(createdUser.ID));
            });
        }

        [Test]
        public async Task UpdateUser_OnlyUsername_ShouldUpdateUsernameOnly()
        {
            var createUserDto = new CreateUserDTO("testuser", "test@email.com", "Password123!");
            var registerResponse = await _client.PostAsJsonAsync("/api/users/register", createUserDto);
            var createdUser = await GetUserFromResponse(registerResponse);

            var updateDto = new UpdateUserDTO("newusername", null);

            var updateResponse = await _client.PutAsJsonAsync($"/api/users/{createdUser.ID}", updateDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var updatedUser = await GetUserFromResponse(updateResponse);
            Assert.Multiple(() =>
            {
                Assert.That(updatedUser.Username, Is.EqualTo("newusername"));
                Assert.That(updatedUser.Email, Is.EqualTo("test@email.com"));
            });
        }

        [Test]
        public async Task UpdateUser_OnlyEmail_ShouldUpdateEmailOnly()
        {
            var createUserDto = new CreateUserDTO("testuser", "old@email.com", "Password123!");
            var registerResponse = await _client.PostAsJsonAsync("/api/users/register", createUserDto);
            var createdUser = await GetUserFromResponse(registerResponse);

            var updateDto = new UpdateUserDTO(null, "new@email.com");

            var updateResponse = await _client.PutAsJsonAsync($"/api/users/{createdUser.ID}", updateDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var updatedUser = await GetUserFromResponse(updateResponse);
            Assert.Multiple(() =>
            {
                Assert.That(updatedUser.Username, Is.EqualTo("testuser"));
                Assert.That(updatedUser.Email, Is.EqualTo("new@email.com"));
            });
        }

        [Test]
        public async Task UpdateUser_WithNonExistentUser_ShouldReturnNotFound()
        {
            var updateDto = new UpdateUserDTO("newuser", "new@email.com");
            var nonExistentId = 99999;

            var updateResponse = await _client.PutAsJsonAsync($"/api/users/{nonExistentId}", updateDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task UpdateUser_WithEmptyData_ShouldReturnBadRequest()
        {
            var createUserDto = new CreateUserDTO("testuser", "test@email.com", "Password123!");
            var registerResponse = await _client.PostAsJsonAsync("/api/users/register", createUserDto);
            var createdUser = await GetUserFromResponse(registerResponse);

            var updateDto = new UpdateUserDTO(null, null);

            var updateResponse = await _client.PutAsJsonAsync($"/api/users/{createdUser.ID}", updateDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }


        [Test]
        public async Task ChangePassword_WithValidData_ShouldChangeSuccessfully()
        {
            var originalPassword = "OriginalPassword123!";
            var newPassword = "NewPassword456!";
            var createUserDto = new CreateUserDTO("testuser", "test@email.com", originalPassword);
            var registerResponse = await _client.PostAsJsonAsync("/api/users/register", createUserDto);
            var createdUser = await GetUserFromResponse(registerResponse);

            var changePasswordDto = new ChangePasswordDTO(originalPassword, newPassword);

            var changeResponse = await _client.PutAsJsonAsync($"/api/users/{createdUser.ID}/change-password", changePasswordDto);

            Assert.That(changeResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var oldLoginResponse = await _client.PostAsJsonAsync("/api/users/login",
                new LoginUserDTO(createdUser.Username, originalPassword));
            Assert.That(oldLoginResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

            var newLoginResponse = await _client.PostAsJsonAsync("/api/users/login",
                new LoginUserDTO(createdUser.Username, newPassword));
            Assert.That(newLoginResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task ChangePassword_WithIncorrectCurrentPassword_ShouldReturnUnauthorized()
        {
            var originalPassword = "OriginalPassword123!";
            var wrongCurrentPassword = "WrongPassword123!";
            var newPassword = "NewPassword456!";

            var createUserDto = new CreateUserDTO("testuser", "test@email.com", originalPassword);
            var registerResponse = await _client.PostAsJsonAsync("/api/users/register", createUserDto);
            var createdUser = await GetUserFromResponse(registerResponse);

            var changePasswordDto = new ChangePasswordDTO(wrongCurrentPassword, newPassword);

            var changeResponse = await _client.PutAsJsonAsync($"/api/users/{createdUser.ID}/change-password", changePasswordDto);

            Assert.That(changeResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task ChangePassword_WithSamePassword_ShouldReturnBadRequest()
        {
            var password = "SamePassword123!";
            var createUserDto = new CreateUserDTO("testuser", "test@email.com", password);
            var registerResponse = await _client.PostAsJsonAsync("/api/users/register", createUserDto);
            var createdUser = await GetUserFromResponse(registerResponse);

            var changePasswordDto = new ChangePasswordDTO(password, password);

            var changeResponse = await _client.PutAsJsonAsync($"/api/users/{createdUser.ID}/change-password", changePasswordDto);

            Assert.That(changeResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task ChangePassword_WithShortPassword_ShouldReturnBadRequest()
        {
            var originalPassword = "OriginalPassword123!";
            var shortPassword = "12345";

            var createUserDto = new CreateUserDTO("testuser", "test@email.com", originalPassword);
            var registerResponse = await _client.PostAsJsonAsync("/api/users/register", createUserDto);
            var createdUser = await GetUserFromResponse(registerResponse);

            var changePasswordDto = new ChangePasswordDTO(originalPassword, shortPassword);

            var changeResponse = await _client.PutAsJsonAsync($"/api/users/{createdUser.ID}/change-password", changePasswordDto);

            Assert.That(changeResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task ChangePassword_WithNonExistentUser_ShouldReturnNotFound()
        {
            var changePasswordDto = new ChangePasswordDTO("CurrentPassword123!", "NewPassword456!");
            var nonExistentId = 99999;

            var changeResponse = await _client.PutAsJsonAsync($"/api/users/{nonExistentId}/change-password", changePasswordDto);

            Assert.That(changeResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
        private static async Task<UserDTO> GetUserFromResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserDTO>(content, _jsonOptions)!;
        }
    }
}