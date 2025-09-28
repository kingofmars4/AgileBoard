using AgileBoard.API.DTOs;
using AgileBoard.Infrastructure;
using AgileBoard.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace AgileBoard.Tests.Base
{
    [TestFixture]
    public abstract class AuthenticatedTestBase
    {
        protected TestWebApplicationFactory<API.Program> _factory;
        protected HttpClient _client;
        protected string _currentToken;
        protected UserDTO _currentUser;
        
        private UserDTO _userBackup;

        [OneTimeSetUp]
        public virtual void OneTimeSetUp()
        {
            _factory = new TestWebApplicationFactory<API.Program>();
            _client = _factory.CreateClient();
        }

        [SetUp]
        public virtual async Task SetUp()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AgileBoardDbContext>();

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            AuthenticationHelper.ClearAuthorizationHeader(_client);
            
            await CreateAuthenticatedUserAsync();
        }

        [OneTimeTearDown]
        public virtual void OneTimeTearDown()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        protected virtual async Task CreateAuthenticatedUserAsync(
            string username = "testuser", 
            string email = "test@example.com", 
            string password = "TestPassword123!")
        {
            var (token, user) = await AuthenticationHelper.CreateAndLoginUserAsync(_client, username, email, password);
            _currentToken = token;
            _currentUser = user;
            _userBackup = user;
            AuthenticationHelper.SetAuthorizationHeader(_client, token);
        }

        protected async Task<UserDTO> CreateAdditionalUserAsync(
            string username = "otheruser", 
            string email = "other@example.com", 
            string password = "OtherPassword123!")
        {
            var createUserDto = new CreateUserDTO(username, email, password);
            var registerResponse = await _client.PostAsJsonAsync("/api/users/register", createUserDto);
            
            if (!registerResponse.IsSuccessStatusCode)
            {
                var error = await registerResponse.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create additional user: {error}");
            }

            return await GetUserFromResponse(registerResponse);
        }

        protected async Task SwitchToUserAsync(string username, string password)
        {
            AuthenticationHelper.ClearAuthorizationHeader(_client);
            
            var loginDto = new LoginUserDTO(username, password);
            var loginResponse = await _client.PostAsJsonAsync("/api/users/login", loginDto);
            
            if (!loginResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to switch to user {username}");
            }

            var loginContent = await loginResponse.Content.ReadAsStringAsync();
            var loginResult = JsonSerializer.Deserialize<LoginResponseDTO>(loginContent, JsonOptions);
            
            _currentToken = loginResult!.Token;
            _currentUser = loginResult.User;
            AuthenticationHelper.SetAuthorizationHeader(_client, _currentToken);
        }

        protected void ClearAuthentication()
        {
            AuthenticationHelper.ClearAuthorizationHeader(_client);
            _currentToken = string.Empty;
        }

        protected UserDTO GetUserData()
        {
            return _currentUser ?? _userBackup;
        }

        protected static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        protected static async Task<UserDTO> GetUserFromResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserDTO>(content, JsonOptions)!;
        }
    }
}