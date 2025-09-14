using AgileBoard.API.DTOs;
using AgileBoard.Tests.Base;
using System.Net;
using System.Text.Json;

namespace AgileBoard.Tests.Users
{
    [TestFixture]
    public class UserFunctionsTests : AuthenticatedTestBase
    {
        [Test]
        public async Task UpdateUser_WithValidData_ShouldUpdateSuccessfully()
        {
            var updateDto = new UpdateUserDTO("updateduser", "updated@email.com");
            var updateResponse = await _client.PutAsJsonAsync($"/api/users/{_currentUser.Id}", updateDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var updatedUser = await GetUserFromResponse(updateResponse);
            Assert.Multiple(() =>
            {
                Assert.That(updatedUser.Username, Is.EqualTo("updateduser"));
                Assert.That(updatedUser.Email, Is.EqualTo("updated@email.com"));
                Assert.That(updatedUser.Id, Is.EqualTo(_currentUser.Id));
            });
        }

        [Test]
        public async Task UpdateUser_OnlyUsername_ShouldUpdateUsernameOnly()
        {
            var updateDto = new UpdateUserDTO("newusername", null);
            var updateResponse = await _client.PutAsJsonAsync($"/api/users/{_currentUser.Id}", updateDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var updatedUser = await GetUserFromResponse(updateResponse);
            Assert.Multiple(() =>
            {
                Assert.That(updatedUser.Username, Is.EqualTo("newusername"));
                Assert.That(updatedUser.Email, Is.EqualTo(_currentUser.Email));
            });
        }

        [Test]
        public async Task UpdateUser_OnlyEmail_ShouldUpdateEmailOnly()
        {
            var updateDto = new UpdateUserDTO(null, "new@email.com");
            var updateResponse = await _client.PutAsJsonAsync($"/api/users/{_currentUser.Id}", updateDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var updatedUser = await GetUserFromResponse(updateResponse);
            Assert.Multiple(() =>
            {
                Assert.That(updatedUser.Username, Is.EqualTo(_currentUser.Username));
                Assert.That(updatedUser.Email, Is.EqualTo("new@email.com"));
            });
        }

        [Test]
        public async Task UpdateUser_WithNonExistentUser_ShouldReturnForbidden()
        {
            var updateDto = new UpdateUserDTO("newuser", "new@email.com");
            var nonExistentId = 99999;

            var updateResponse = await _client.PutAsJsonAsync($"/api/users/{nonExistentId}", updateDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task UpdateUser_WithEmptyData_ShouldReturnBadRequest()
        {
            var updateDto = new UpdateUserDTO(null, null);
            var updateResponse = await _client.PutAsJsonAsync($"/api/users/{_currentUser.Id}", updateDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task ChangePassword_WithValidData_ShouldChangeSuccessfully()
        {
            var originalPassword = "TestPassword123!";
            var newPassword = "NewPassword456!";

            var username = _currentUser.Username;
            var userId = _currentUser.Id;

            var changePasswordDto = new ChangePasswordDTO(originalPassword, newPassword);
            var changeResponse = await _client.PutAsJsonAsync($"/api/users/{userId}/change-password", changePasswordDto);

            Assert.That(changeResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            ClearAuthentication();

            var oldLoginResponse = await _client.PostAsJsonAsync("/api/users/login",
                new LoginUserDTO(username, originalPassword));
            Assert.That(oldLoginResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

            var newLoginResponse = await _client.PostAsJsonAsync("/api/users/login",
                new LoginUserDTO(username, newPassword));
            Assert.That(newLoginResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task ChangePassword_WithIncorrectCurrentPassword_ShouldReturnUnauthorized()
        {
            var wrongCurrentPassword = "WrongPassword123!";
            var newPassword = "NewPassword456!";

            var changePasswordDto = new ChangePasswordDTO(wrongCurrentPassword, newPassword);
            var changeResponse = await _client.PutAsJsonAsync($"/api/users/{_currentUser.Id}/change-password", changePasswordDto);

            Assert.That(changeResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task ChangePassword_WithSamePassword_ShouldReturnBadRequest()
        {
            var password = "TestPassword123!";

            var changePasswordDto = new ChangePasswordDTO(password, password);
            var changeResponse = await _client.PutAsJsonAsync($"/api/users/{_currentUser.Id}/change-password", changePasswordDto);

            Assert.That(changeResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task ChangePassword_WithShortPassword_ShouldReturnBadRequest()
        {
            var originalPassword = "TestPassword123!";
            var shortPassword = "12345";

            var changePasswordDto = new ChangePasswordDTO(originalPassword, shortPassword);
            var changeResponse = await _client.PutAsJsonAsync($"/api/users/{_currentUser.Id}/change-password", changePasswordDto);

            Assert.That(changeResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task ChangePassword_WithNonExistentUser_ShouldReturnForbidden()
        {
            var changePasswordDto = new ChangePasswordDTO("CurrentPassword123!", "NewPassword456!");
            var nonExistentId = 99999;

            var changeResponse = await _client.PutAsJsonAsync($"/api/users/{nonExistentId}/change-password", changePasswordDto);

            Assert.That(changeResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task UpdateUser_TryToUpdateOtherUser_ShouldReturnForbidden()
        {
            var otherUser = await CreateAdditionalUserAsync("otheruser", "other@example.com");

            var updateDto = new UpdateUserDTO("hackeduser", "hacked@email.com");
            var updateResponse = await _client.PutAsJsonAsync($"/api/users/{otherUser.Id}", updateDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task GetCurrentUser_ShouldReturnCurrentUserData()
        {
            var response = await _client.GetAsync("/api/users/me");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            
            var actualUser = await GetUserFromResponse(response);
            Assert.Multiple(() =>
            {
                Assert.That(actualUser.Id, Is.EqualTo(_currentUser.Id));
                Assert.That(actualUser.Username, Is.EqualTo(_currentUser.Username));
                Assert.That(actualUser.Email, Is.EqualTo(_currentUser.Email));
            });
        }

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
    }
}