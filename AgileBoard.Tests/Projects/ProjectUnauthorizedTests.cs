using AgileBoard.API.DTOs;
using AgileBoard.Tests.Base;
using System.Net;

namespace AgileBoard.Tests.Projects
{
    [TestFixture]
    public class ProjectUnauthorizedTests : AuthenticatedTestBase
    {
        [Test]
        public async Task CreateProject_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            ClearAuthentication();

            var createProjectDto = new CreateProjectDTO("Test Project", "Description");
            var createResponse = await _client.PostAsJsonAsync("/api/project", createProjectDto);

            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GetAllProjects_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            ClearAuthentication();

            var getResponse = await _client.GetAsync("/api/project");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GetProjectById_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            ClearAuthentication();

            var getResponse = await _client.GetAsync("/api/project/1");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task UpdateProject_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            ClearAuthentication();

            var updateProjectDto = new UpdateProjectDTO("Updated Name", "Updated Description");
            var updateResponse = await _client.PutAsJsonAsync("/api/project/1", updateProjectDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task DeleteProject_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            ClearAuthentication();

            var deleteResponse = await _client.DeleteAsync("/api/project/1");

            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GetProjectById_AsNonOwnerNonParticipant_ShouldReturnForbidden()
        {
            var createProjectDto = new CreateProjectDTO("Private Project", "Only for owner");
            var createResponse = await _client.PostAsJsonAsync("/api/project", createProjectDto);
            var createdProject = await GetProjectFromResponse(createResponse);

            await CreateAdditionalUserAsync("user2", "user2@example.com", "Password123!");
            await SwitchToUserAsync("user2", "Password123!");

            var getResponse = await _client.GetAsync($"/api/project/{createdProject.Id}");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task UpdateProject_AsNonOwner_ShouldReturnForbidden()
        {
            var createProjectDto = new CreateProjectDTO("Update Test Project", "Description");
            var createResponse = await _client.PostAsJsonAsync("/api/project", createProjectDto);
            var createdProject = await GetProjectFromResponse(createResponse);

            await CreateAdditionalUserAsync("user2", "user2@example.com", "Password123!");
            await SwitchToUserAsync("user2", "Password123!");

            var updateProjectDto = new UpdateProjectDTO("Hacked Name", "Hacked Description");
            var updateResponse = await _client.PutAsJsonAsync($"/api/project/{createdProject.Id}", updateProjectDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task DeleteProject_AsNonOwner_ShouldReturnForbidden()
        {
            var createProjectDto = new CreateProjectDTO("Delete Test Project", "Description");
            var createResponse = await _client.PostAsJsonAsync("/api/project", createProjectDto);
            var createdProject = await GetProjectFromResponse(createResponse);

            await CreateAdditionalUserAsync("user2", "user2@example.com", "Password123!");
            await SwitchToUserAsync("user2", "Password123!");

            var deleteResponse = await _client.DeleteAsync($"/api/project/{createdProject.Id}");

            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task AddParticipant_AsNonOwner_ShouldReturnForbidden()
        {
            var createProjectDto = new CreateProjectDTO("Participant Test", "Description");
            var createResponse = await _client.PostAsJsonAsync("/api/project", createProjectDto);
            var createdProject = await GetProjectFromResponse(createResponse);

            await CreateAdditionalUserAsync("user2", "user2@example.com", "Password123!");
            var user3 = await CreateAdditionalUserAsync("user3", "user3@example.com", "Password123!");

            await SwitchToUserAsync("user2", "Password123!");

            var addParticipantDto = new AddParticipantDTO(user3.Id);
            var addResponse = await _client.PostAsJsonAsync($"/api/project/{createdProject.Id}/participants", addParticipantDto);

            Assert.That(addResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task RemoveParticipant_AsNonOwner_ShouldReturnForbidden()
        {
            var createProjectDto = new CreateProjectDTO("Remove Participant Test", "Description");
            var createResponse = await _client.PostAsJsonAsync("/api/project", createProjectDto);
            var createdProject = await GetProjectFromResponse(createResponse);

            var user2 = await CreateAdditionalUserAsync("user2", "user2@example.com", "Password123!");
            await CreateAdditionalUserAsync("user3", "user3@example.com", "Password123!");

            var addParticipantDto = new AddParticipantDTO(user2.Id);
            await _client.PostAsJsonAsync($"/api/project/{createdProject.Id}/participants", addParticipantDto);

            await SwitchToUserAsync("user3", "Password123!");

            var removeResponse = await _client.DeleteAsync($"/api/project/{createdProject.Id}/participants/{user2.Id}");

            Assert.That(removeResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task GetProjectById_AsParticipant_ShouldReturnProject()
        {
            var createProjectDto = new CreateProjectDTO("Participant Access Test", "Description");
            var createResponse = await _client.PostAsJsonAsync("/api/project", createProjectDto);
            var createdProject = await GetProjectFromResponse(createResponse);

            var user2 = await CreateAdditionalUserAsync("participant", "participant@example.com", "Password123!");

            var addParticipantDto = new AddParticipantDTO(user2.Id);
            await _client.PostAsJsonAsync($"/api/project/{createdProject.Id}/participants", addParticipantDto);

            await SwitchToUserAsync("participant", "Password123!");

            var getResponse = await _client.GetAsync($"/api/project/{createdProject.Id}");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var project = await GetProjectFromResponse(getResponse);
            Assert.That(project.Id, Is.EqualTo(createdProject.Id));
        }

        [Test]
        public async Task UpdateProject_AsParticipant_ShouldReturnForbidden()
        {
            var createProjectDto = new CreateProjectDTO("Participant Update Test", "Description");
            var createResponse = await _client.PostAsJsonAsync("/api/project", createProjectDto);
            var createdProject = await GetProjectFromResponse(createResponse);

            var user2 = await CreateAdditionalUserAsync("participant", "participant@example.com", "Password123!");

            var addParticipantDto = new AddParticipantDTO(user2.Id);
            await _client.PostAsJsonAsync($"/api/project/{createdProject.Id}/participants", addParticipantDto);

            await SwitchToUserAsync("participant", "Password123!");

            var updateProjectDto = new UpdateProjectDTO("Updated by Participant", "Should not work");
            var updateResponse = await _client.PutAsJsonAsync($"/api/project/{createdProject.Id}", updateProjectDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task CompleteProjectFlow_WithMultipleUsers()
        {
            var createProjectDto = new CreateProjectDTO("Multi-User Test", "Testing access levels");
            var createResponse = await _client.PostAsJsonAsync("/api/project", createProjectDto);
            var createdProject = await GetProjectFromResponse(createResponse);

            var user2 = await CreateAdditionalUserAsync("user2", "user2@example.com", "Password123!");
            await CreateAdditionalUserAsync("user3", "user3@example.com", "Password123!");

            await _client.PostAsJsonAsync($"/api/project/{createdProject.Id}/participants", new AddParticipantDTO(user2.Id));

            await SwitchToUserAsync("user2", "Password123!");
            var getResponse2 = await _client.GetAsync($"/api/project/{createdProject.Id}");
            Assert.That(getResponse2.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            await SwitchToUserAsync("user3", "Password123!");
            var getResponse3 = await _client.GetAsync($"/api/project/{createdProject.Id}");
            Assert.That(getResponse3.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

            await SwitchToUserAsync("user2", "Password123!");
            var updateResponse = await _client.PutAsJsonAsync($"/api/project/{createdProject.Id}", 
                new UpdateProjectDTO("Should fail", "Participant cannot update"));
            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        private static async Task<ProjectDTO> GetProjectFromResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<ProjectDTO>(content, JsonOptions)!;
        }
    }
}