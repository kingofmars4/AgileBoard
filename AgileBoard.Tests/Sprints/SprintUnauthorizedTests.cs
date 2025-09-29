using AgileBoard.API.DTOs;
using AgileBoard.Tests.Base;
using System.Net;

namespace AgileBoard.Tests.Sprints
{
    [TestFixture]
    public class SprintUnauthorizedTests : AuthenticatedTestBase
    {
        private ProjectDTO _testProject;

        [SetUp]
        public override async Task SetUp()
        {
            await base.SetUp();
            
            var createProjectDto = new CreateProjectDTO("Test Project", "Project for sprint tests");
            var projectResponse = await _client.PostAsJsonAsync("/api/project", createProjectDto);
            _testProject = await GetProjectFromResponse(projectResponse);
        }

        [Test]
        public async Task CreateSprint_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            ClearAuthentication();

            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = startDate.AddDays(14);
            
            var createSprintDto = new CreateSprintDTO("Test Sprint", "Description", _testProject.Id, startDate, endDate);
            var createResponse = await _client.PostAsJsonAsync("/api/sprint", createSprintDto);

            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GetAllSprints_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            ClearAuthentication();

            var getResponse = await _client.GetAsync("/api/sprint");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GetSprintById_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            ClearAuthentication();

            var getResponse = await _client.GetAsync("/api/sprint/1");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task UpdateSprint_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            ClearAuthentication();

            var updateSprintDto = new UpdateSprintDTO("Updated Name", "Updated Description", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(15));
            var updateResponse = await _client.PutAsJsonAsync("/api/sprint/1", updateSprintDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task DeleteSprint_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            ClearAuthentication();

            var deleteResponse = await _client.DeleteAsync("/api/sprint/1");

            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task CreateSprint_InProjectUserDoesntOwn_ShouldReturnForbidden()
        {
            await CreateAdditionalUserAsync("user2", "user2@example.com", "Password123!");
            await SwitchToUserAsync("user2", "Password123!");

            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = startDate.AddDays(14);
            
            var createSprintDto = new CreateSprintDTO("Unauthorized Sprint", "Should not create", _testProject.Id, startDate, endDate);
            var createResponse = await _client.PostAsJsonAsync("/api/sprint", createSprintDto);

            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task GetSprintById_FromProjectUserCantAccess_ShouldReturnForbidden()
        {
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = startDate.AddDays(14);
            
            var createSprintDto = new CreateSprintDTO("Private Sprint", "Only for owner", _testProject.Id, startDate, endDate);
            var createResponse = await _client.PostAsJsonAsync("/api/sprint", createSprintDto);
            var createdSprint = await GetSprintFromResponse(createResponse);

            await CreateAdditionalUserAsync("user2", "user2@example.com", "Password123!");
            await SwitchToUserAsync("user2", "Password123!");

            var getResponse = await _client.GetAsync($"/api/sprint/{createdSprint.Id}");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task UpdateSprint_AsNonProjectMember_ShouldReturnForbidden()
        {
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = startDate.AddDays(14);
            
            var createSprintDto = new CreateSprintDTO("Update Test Sprint", "Description", _testProject.Id, startDate, endDate);
            var createResponse = await _client.PostAsJsonAsync("/api/sprint", createSprintDto);
            var createdSprint = await GetSprintFromResponse(createResponse);

            await CreateAdditionalUserAsync("user2", "user2@example.com", "Password123!");
            await SwitchToUserAsync("user2", "Password123!");

            var updateSprintDto = new UpdateSprintDTO("Hacked Name", "Hacked Description", null, null);
            var updateResponse = await _client.PutAsJsonAsync($"/api/sprint/{createdSprint.Id}", updateSprintDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task DeleteSprint_AsNonOwner_ShouldReturnForbidden()
        {
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = startDate.AddDays(14);
            
            var createSprintDto = new CreateSprintDTO("Delete Test Sprint", "Description", _testProject.Id, startDate, endDate);
            var createResponse = await _client.PostAsJsonAsync("/api/sprint", createSprintDto);
            var createdSprint = await GetSprintFromResponse(createResponse);

            await CreateAdditionalUserAsync("user2", "user2@example.com", "Password123!");
            await SwitchToUserAsync("user2", "Password123!");

            var deleteResponse = await _client.DeleteAsync($"/api/sprint/{createdSprint.Id}");

            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task GetSprintById_AsProjectParticipant_ShouldReturnSprint()
        {
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = startDate.AddDays(14);
            
            var createSprintDto = new CreateSprintDTO("Participant Access Test", "Description", _testProject.Id, startDate, endDate);
            var createResponse = await _client.PostAsJsonAsync("/api/sprint", createSprintDto);
            var createdSprint = await GetSprintFromResponse(createResponse);

            var user2 = await CreateAdditionalUserAsync("participant", "participant@example.com", "Password123!");

            await _client.PostAsJsonAsync($"/api/project/{_testProject.Id}/participants", new AddParticipantDTO(user2.Id));

            await SwitchToUserAsync("participant", "Password123!");

            var getResponse = await _client.GetAsync($"/api/sprint/{createdSprint.Id}");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var sprint = await GetSprintFromResponse(getResponse);
            Assert.That(sprint.Id, Is.EqualTo(createdSprint.Id));
        }

        [Test]
        public async Task UpdateSprint_AsProjectParticipant_ShouldUpdateSuccessfully()
        {
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = startDate.AddDays(14);
            
            var createSprintDto = new CreateSprintDTO("Participant Update Test", "Description", _testProject.Id, startDate, endDate);
            var createResponse = await _client.PostAsJsonAsync("/api/sprint", createSprintDto);
            var createdSprint = await GetSprintFromResponse(createResponse);

            var user2 = await CreateAdditionalUserAsync("participant", "participant@example.com", "Password123!");

            await _client.PostAsJsonAsync($"/api/project/{_testProject.Id}/participants", new AddParticipantDTO(user2.Id));

            await SwitchToUserAsync("participant", "Password123!");

            var updateSprintDto = new UpdateSprintDTO("Updated by Participant", "Updated description", null, null);
            var updateResponse = await _client.PutAsJsonAsync($"/api/sprint/{createdSprint.Id}", updateSprintDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var updatedSprint = await GetSprintFromResponse(updateResponse);
            Assert.That(updatedSprint.Name, Is.EqualTo("Updated by Participant"));
        }

        [Test]
        public async Task GetSprintsByProject_AsNonProjectMember_ShouldReturnForbidden()
        {
            await CreateAdditionalUserAsync("user2", "user2@example.com", "Password123!");
            await SwitchToUserAsync("user2", "Password123!");

            var getResponse = await _client.GetAsync($"/api/sprint/project/{_testProject.Id}");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task CompleteSprintFlow_WithMultipleUsers()
        {
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = startDate.AddDays(14);
            
            var createSprintDto = new CreateSprintDTO("Multi-User Test", "Testing access levels", _testProject.Id, startDate, endDate);
            var createResponse = await _client.PostAsJsonAsync("/api/sprint", createSprintDto);
            var createdSprint = await GetSprintFromResponse(createResponse);

            var user2 = await CreateAdditionalUserAsync("user2", "user2@example.com", "Password123!");
            var user3 = await CreateAdditionalUserAsync("user3", "user3@example.com", "Password123!");

            await _client.PostAsJsonAsync($"/api/project/{_testProject.Id}/participants", new AddParticipantDTO(user2.Id));

            await SwitchToUserAsync("user2", "Password123!");
            var getResponse2 = await _client.GetAsync($"/api/sprint/{createdSprint.Id}");
            Assert.That(getResponse2.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            await SwitchToUserAsync("user3", "Password123!");
            var getResponse3 = await _client.GetAsync($"/api/sprint/{createdSprint.Id}");
            Assert.That(getResponse3.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

            await SwitchToUserAsync("user2", "Password123!");
            var updateResponse = await _client.PutAsJsonAsync($"/api/sprint/{createdSprint.Id}", 
                new UpdateSprintDTO("Updated by participant", null, null, null));
            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var deleteResponse = await _client.DeleteAsync($"/api/sprint/{createdSprint.Id}");
            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        private static async Task<SprintDTO> GetSprintFromResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<SprintDTO>(content, JsonOptions)!;
        }

        private static async Task<ProjectDTO> GetProjectFromResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<ProjectDTO>(content, JsonOptions)!;
        }
    }
}