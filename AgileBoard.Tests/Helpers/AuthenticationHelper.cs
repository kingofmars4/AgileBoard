using AgileBoard.API.DTOs;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AgileBoard.Tests.Helpers
{
    public static class AuthenticationHelper
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static async Task<(string Token, UserDTO User)> CreateAndLoginUserAsync(
            HttpClient client, 
            string username = "testuser", 
            string email = "test@example.com", 
            string password = "TestPassword123!")
        {
            var createUserDto = new CreateUserDTO(username, email, password);
            var registerResponse = await client.PostAsJsonAsync("/api/users/register", createUserDto);
            
            if (!registerResponse.IsSuccessStatusCode)
            {
                var errorContent = await registerResponse.Content.ReadAsStringAsync();
                throw new Exception($"Failed to register user: {registerResponse.StatusCode} - {errorContent}");
            }

            var loginDto = new LoginUserDTO(username, password);
            var loginResponse = await client.PostAsJsonAsync("/api/users/login", loginDto);
            
            if (!loginResponse.IsSuccessStatusCode)
            {
                var errorContent = await loginResponse.Content.ReadAsStringAsync();
                throw new Exception($"Failed to login: {loginResponse.StatusCode} - {errorContent}");
            }

            var loginContent = await loginResponse.Content.ReadAsStringAsync();
            var loginResult = JsonSerializer.Deserialize<LoginResponseDTO>(loginContent, _jsonOptions);

            return (loginResult!.Token, loginResult.User);
        }

        public static void SetAuthorizationHeader(HttpClient client, string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public static void ClearAuthorizationHeader(HttpClient client)
        {
            client.DefaultRequestHeaders.Authorization = null;
        }
    }
}