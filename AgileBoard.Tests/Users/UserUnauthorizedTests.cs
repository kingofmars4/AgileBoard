using AgileBoard.API.DTOs;
using AgileBoard.Tests.Base;
using System.Net;

namespace AgileBoard.Tests.Users
{
    [TestFixture]
    public class UserUnauthorizedTests : AuthenticatedTestBase
    {
        [Test]
        public async Task UpdateUser_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            ClearAuthentication();
            
            var updateDto = new UpdateUserDTO("newuser", "new@email.com");
            var updateResponse = await _client.PutAsJsonAsync($"/api/users/{_currentUser.Id}", updateDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task ChangePassword_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            ClearAuthentication();
            
            var changePasswordDto = new ChangePasswordDTO("CurrentPassword123!", "NewPassword456!");
            var changeResponse = await _client.PutAsJsonAsync($"/api/users/{_currentUser.Id}/change-password", changePasswordDto);

            Assert.That(changeResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GetCurrentUser_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            ClearAuthentication();
            
            var response = await _client.GetAsync("/api/users/me");
            
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GetAllUsers_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            ClearAuthentication();
            
            var response = await _client.GetAsync("/api/users");
            
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GetUserById_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            ClearAuthentication();
            
            var response = await _client.GetAsync($"/api/users/{_currentUser.Id}");
            
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task UpdateUser_TryToUpdateOtherUser_ShouldReturnForbidden()
        {
            var otherUser = await CreateAdditionalUserAsync("otheruser", "other@example.com", "OtherPassword123!");

            var updateDto = new UpdateUserDTO("hackeduser", "hacked@email.com");
            var updateResponse = await _client.PutAsJsonAsync($"/api/users/{otherUser.Id}", updateDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task ChangePassword_TryToChangeOtherUserPassword_ShouldReturnForbidden()
        {
            var otherUser = await CreateAdditionalUserAsync("otheruser", "other@example.com", "OtherPassword123!");

            var changePasswordDto = new ChangePasswordDTO("OtherPassword123!", "HackedPassword456!");
            var changeResponse = await _client.PutAsJsonAsync($"/api/users/{otherUser.Id}/change-password", changePasswordDto);

            Assert.That(changeResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task UpdateUser_WithInvalidToken_ShouldReturnUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "invalid.jwt.token");

            var updateDto = new UpdateUserDTO("newuser", "new@email.com");
            var updateResponse = await _client.PutAsJsonAsync($"/api/users/{_currentUser.Id}", updateDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task UpdateUser_WithExpiredToken_ShouldReturnUnauthorized()
        {
            var expiredToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.invalid";
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", expiredToken);

            var updateDto = new UpdateUserDTO("newuser", "new@email.com");
            var updateResponse = await _client.PutAsJsonAsync($"/api/users/{_currentUser.Id}", updateDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task MultipleUsersScenario_UserCannotAccessOtherUsersData()
        {
            var user2 = await CreateAdditionalUserAsync("user2", "user2@example.com", "User2Password123!");
            
            var user3 = await CreateAdditionalUserAsync("user3", "user3@example.com", "User3Password123!");

            var updateUser2Dto = new UpdateUserDTO("hackeduser2", "hacked2@email.com");
            var updateUser2Response = await _client.PutAsJsonAsync($"/api/users/{user2.Id}", updateUser2Dto);
            Assert.That(updateUser2Response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

            var updateUser3Dto = new UpdateUserDTO("hackeduser3", "hacked3@email.com");
            var updateUser3Response = await _client.PutAsJsonAsync($"/api/users/{user3.Id}", updateUser3Dto);
            Assert.That(updateUser3Response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

            var updateUser1Dto = new UpdateUserDTO("updateduser1", "updated1@email.com");
            var updateUser1Response = await _client.PutAsJsonAsync($"/api/users/{_currentUser.Id}", updateUser1Dto);
            Assert.That(updateUser1Response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task SwitchUsers_EachUserCanOnlyModifyThemselves()
        {
            var updateUser1Dto = new UpdateUserDTO("user1updated", "user1updated@email.com");
            var updateUser1Response = await _client.PutAsJsonAsync($"/api/users/{_currentUser.Id}", updateUser1Dto);
            Assert.That(updateUser1Response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var user1Id = _currentUser.Id;

            await CreateAdditionalUserAsync("user2", "user2@example.com", "User2Password123!");
            await SwitchToUserAsync("user2", "User2Password123!");

            var hackUser1Dto = new UpdateUserDTO("hackeduser1", "hacked1@email.com");
            var hackUser1Response = await _client.PutAsJsonAsync($"/api/users/{user1Id}", hackUser1Dto);
            Assert.That(hackUser1Response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

            var updateUser2Dto = new UpdateUserDTO("user2updated", "user2updated@email.com");
            var updateUser2Response = await _client.PutAsJsonAsync($"/api/users/{_currentUser.Id}", updateUser2Dto);
            Assert.That(updateUser2Response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task AuthenticationFlow_CompleteSecurityTest()
        {

            var originalUserId = _currentUser.Id;

            var meResponse1 = await _client.GetAsync("/api/users/me");
            Assert.That(meResponse1.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            ClearAuthentication();
            var meResponse2 = await _client.GetAsync("/api/users/me");
            Assert.That(meResponse2.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "fake.token.here");
            var meResponse3 = await _client.GetAsync("/api/users/me");
            Assert.That(meResponse3.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

            await CreateAuthenticatedUserAsync("AuthenticationFlowUser", "test@example.com", "TestPassword123!");
            var meResponse4 = await _client.GetAsync("/api/users/me");
            Assert.That(meResponse4.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task InvalidJwtFormats_ShouldReturnUnauthorized()
        {

            var invalidTokens = new[]
            {
                "not.a.jwt",
                "Bearer invalid",
                "completely-invalid-token",
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.invalid",
                "",
                "null"
            };

            foreach (var invalidToken in invalidTokens)
            {
                _client.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", invalidToken);

                var response = await _client.GetAsync("/api/users/me");
                
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized), 
                    $"Token '{invalidToken}' should return Unauthorized");
            }
        }
    }
}
