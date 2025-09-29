using AgileBoard.API.DTOs;
using AgileBoard.Tests.Base;
using System.Net;
using System.Text.Json;

namespace AgileBoard.Tests.Projects
{
    [TestFixture]
    public class ProjectFunctionsTests : AuthenticatedTestBase
    {
        [Test]
        public async Task CreateProject_WithValidData_ShouldCreateSuccessfully()
        {
            var createProjectDto = new CreateProjectDTO("Test Project", "A test project description");
            var createResponse = await _client.PostAsJsonAsync("/api/project", createProjectDto);

            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var createdProject = await GetProjectFromResponse(createResponse);
            Assert.Multiple(() =>
            {
                Assert.That(createdProject.Name, Is.EqualTo("Test Project"));
                Assert.That(createdProject.Description, Is.EqualTo("A test project description"));
                Assert.That(createdProject.Id, Is.GreaterThan(0));
                Assert.That(createdProject.OwnerId, Is.EqualTo(_currentUser.Id));
            });
        }

        [Test]
        public async Task CreateProject_WithEmptyName_ShouldReturnBadRequest()
        {
            var createProjectDto = new CreateProjectDTO("", "Description");
            var createResponse = await _client.PostAsJsonAsync("/api/project", createProjectDto);

            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task CreateProject_WithNullName_ShouldReturnBadRequest()
        {
            var createProjectDto = new CreateProjectDTO(null!, "Description");
            var createResponse = await _client.PostAsJsonAsync("/api/project", createProjectDto);

            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task CreateProject_WithDuplicateName_ShouldReturnConflict()
        {
            var createProjectDto1 = new CreateProjectDTO("Duplicate Project", "First project");
            await _client.PostAsJsonAsync("/api/project", createProjectDto1);

            var createProjectDto2 = new CreateProjectDTO("Duplicate Project", "Second project");
            var createResponse2 = await _client.PostAsJsonAsync("/api/project", createProjectDto2);

            Assert.That(createResponse2.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        }

        [Test]
        public async Task GetProjectById_WithValidId_ShouldReturnProject()
        {
            var createProjectDto = new CreateProjectDTO("Get Test Project", "Description for get test");
            var createResponse = await _client.PostAsJsonAsync("/api/project", createProjectDto);
            var createdProject = await GetProjectFromResponse(createResponse);

            var getResponse = await _client.GetAsync($"/api/project/{createdProject.Id}");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var retrievedProject = await GetProjectFromResponse(getResponse);
            Assert.Multiple(() =>
            {
                Assert.That(retrievedProject.Id, Is.EqualTo(createdProject.Id));
                Assert.That(retrievedProject.Name, Is.EqualTo("Get Test Project"));
                Assert.That(retrievedProject.Description, Is.EqualTo("Description for get test"));
            });
        }

        [Test]
        public async Task GetProjectById_WithNonExistentId_ShouldReturnNotFound()
        {
            var getResponse = await _client.GetAsync("/api/project/99999");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GetAllProjects_WithOwnedProjects_ShouldReturnProjects()
        {
            await _client.PostAsJsonAsync("/api/project", new CreateProjectDTO("Project 1", "Description 1"));
            await _client.PostAsJsonAsync("/api/project", new CreateProjectDTO("Project 2", "Description 2"));
            await _client.PostAsJsonAsync("/api/project", new CreateProjectDTO("Project 3", "Description 3"));

            var getAllResponse = await _client.GetAsync("/api/project");

            Assert.That(getAllResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var projects = await GetProjectSummariesFromResponse(getAllResponse);
            Assert.That(projects.Count(), Is.GreaterThanOrEqualTo(3));

            var projectNames = projects.Select(p => p.Name).ToList();
            Assert.Multiple(() =>
            {
                Assert.That(projectNames, Does.Contain("Project 1"));
                Assert.That(projectNames, Does.Contain("Project 2"));
                Assert.That(projectNames, Does.Contain("Project 3"));
            });
        }

        [Test]
        public async Task GetAllProjects_WithNoProjects_ShouldReturnNotFound()
        {
            var getAllResponse = await _client.GetAsync("/api/project");

            Assert.That(getAllResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task UpdateProject_WithValidData_ShouldUpdateSuccessfully()
        {
            var createProjectDto = new CreateProjectDTO("Original Project", "Original description");
            var createResponse = await _client.PostAsJsonAsync("/api/project", createProjectDto);
            var createdProject = await GetProjectFromResponse(createResponse);

            var updateProjectDto = new UpdateProjectDTO("Updated Project", "Updated description");
            var updateResponse = await _client.PutAsJsonAsync($"/api/project/{createdProject.Id}", updateProjectDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var updatedProject = await GetProjectFromResponse(updateResponse);
            Assert.Multiple(() =>
            {
                Assert.That(updatedProject.Id, Is.EqualTo(createdProject.Id));
                Assert.That(updatedProject.Name, Is.EqualTo("Updated Project"));
                Assert.That(updatedProject.Description, Is.EqualTo("Updated description"));
            });
        }

        [Test]
        public async Task UpdateProject_OnlyName_ShouldUpdateNameOnly()
        {
            var createProjectDto = new CreateProjectDTO("Name Update Test", "Original description");
            var createResponse = await _client.PostAsJsonAsync("/api/project", createProjectDto);
            var createdProject = await GetProjectFromResponse(createResponse);

            var updateProjectDto = new UpdateProjectDTO("New Name Only", null);
            var updateResponse = await _client.PutAsJsonAsync($"/api/project/{createdProject.Id}", updateProjectDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var updatedProject = await GetProjectFromResponse(updateResponse);
            Assert.Multiple(() =>
            {
                Assert.That(updatedProject.Name, Is.EqualTo("New Name Only"));
                Assert.That(updatedProject.Description, Is.EqualTo("Original description"));
            });
        }

        [Test]
        public async Task UpdateProject_OnlyDescription_ShouldUpdateDescriptionOnly()
        {
            var createProjectDto = new CreateProjectDTO("Description Update Test", "Original description");
            var createResponse = await _client.PostAsJsonAsync("/api/project", createProjectDto);
            var createdProject = await GetProjectFromResponse(createResponse);

            var updateProjectDto = new UpdateProjectDTO(null, "New description only");
            var updateResponse = await _client.PutAsJsonAsync($"/api/project/{createdProject.Id}", updateProjectDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var updatedProject = await GetProjectFromResponse(updateResponse);
            Assert.Multiple(() =>
            {
                Assert.That(updatedProject.Name, Is.EqualTo("Description Update Test"));
                Assert.That(updatedProject.Description, Is.EqualTo("New description only"));
            });
        }

        [Test]
        public async Task UpdateProject_WithEmptyData_ShouldReturnBadRequest()
        {
            var createProjectDto = new CreateProjectDTO("Empty Update Test", "Description");
            var createResponse = await _client.PostAsJsonAsync("/api/project", createProjectDto);
            var createdProject = await GetProjectFromResponse(createResponse);

            var updateProjectDto = new UpdateProjectDTO(null, null);
            var updateResponse = await _client.PutAsJsonAsync($"/api/project/{createdProject.Id}", updateProjectDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task UpdateProject_WithNonExistentId_ShouldReturnNotFound()
        {
            var updateProjectDto = new UpdateProjectDTO("Updated Name", "Updated Description");
            var updateResponse = await _client.PutAsJsonAsync("/api/project/99999", updateProjectDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task DeleteProject_WithValidId_ShouldDeleteSuccessfully()
        {
            var createProjectDto = new CreateProjectDTO("Project To Delete", "Will be deleted");
            var createResponse = await _client.PostAsJsonAsync("/api/project", createProjectDto);
            var createdProject = await GetProjectFromResponse(createResponse);

            var deleteResponse = await _client.DeleteAsync($"/api/project/{createdProject.Id}");

            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

            var getResponse = await _client.GetAsync($"/api/project/{createdProject.Id}");
            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task DeleteProject_WithNonExistentId_ShouldReturnNotFound()
        {
            var deleteResponse = await _client.DeleteAsync("/api/project/99999");

            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GetOwnedProjects_ShouldReturnOnlyOwnedProjects()
        {
            await _client.PostAsJsonAsync("/api/project", new CreateProjectDTO("Owned Project 1", "Description 1"));
            await _client.PostAsJsonAsync("/api/project", new CreateProjectDTO("Owned Project 2", "Description 2"));

            var getOwnedResponse = await _client.GetAsync("/api/project/owned");

            Assert.That(getOwnedResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var ownedProjects = await GetProjectSummariesFromResponse(getOwnedResponse);
            Assert.That(ownedProjects.Count(), Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        public async Task AddParticipant_ShouldAddSuccessfully()
        {
            var createProjectDto = new CreateProjectDTO("Participant Test Project", "For testing participants");
            var createResponse = await _client.PostAsJsonAsync("/api/project", createProjectDto);
            var createdProject = await GetProjectFromResponse(createResponse);

            var participantUser = await CreateAdditionalUserAsync("participant", "participant@example.com");

            var addParticipantDto = new AddParticipantDTO(participantUser.Id);
            var addResponse = await _client.PostAsJsonAsync($"/api/project/{createdProject.Id}/participants", addParticipantDto);

            Assert.That(addResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task RemoveParticipant_ShouldRemoveSuccessfully()
        {
            var createProjectDto = new CreateProjectDTO("Remove Participant Test", "For testing participant removal");
            var createResponse = await _client.PostAsJsonAsync("/api/project", createProjectDto);
            var createdProject = await GetProjectFromResponse(createResponse);

            var participantUser = await CreateAdditionalUserAsync("participant2", "participant2@example.com");

            var addParticipantDto = new AddParticipantDTO(participantUser.Id);
            await _client.PostAsJsonAsync($"/api/project/{createdProject.Id}/participants", addParticipantDto);

            var removeResponse = await _client.DeleteAsync($"/api/project/{createdProject.Id}/participants/{participantUser.Id}");

            Assert.That(removeResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task GetProjectParticipants_ShouldReturnParticipants()
        {
            var createProjectDto = new CreateProjectDTO("Get Participants Test", "For testing get participants");
            var createResponse = await _client.PostAsJsonAsync("/api/project", createProjectDto);
            var createdProject = await GetProjectFromResponse(createResponse);

            var participant1 = await CreateAdditionalUserAsync("participant1", "p1@example.com");
            var participant2 = await CreateAdditionalUserAsync("participant2", "p2@example.com");

            await _client.PostAsJsonAsync($"/api/project/{createdProject.Id}/participants", new AddParticipantDTO(participant1.Id));
            await _client.PostAsJsonAsync($"/api/project/{createdProject.Id}/participants", new AddParticipantDTO(participant2.Id));

            var getParticipantsResponse = await _client.GetAsync($"/api/project/{createdProject.Id}/participants");

            Assert.That(getParticipantsResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var participants = await GetUsersFromResponse(getParticipantsResponse);
            Assert.That(participants.Count(), Is.GreaterThanOrEqualTo(2));

            var participantUsernames = participants.Select(p => p.Username).ToList();
            Assert.Multiple(() =>
            {
                Assert.That(participantUsernames, Does.Contain("participant1"));
                Assert.That(participantUsernames, Does.Contain("participant2"));
            });
        }

        private static async Task<ProjectDTO> GetProjectFromResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProjectDTO>(content, JsonOptions)!;
        }

        private static async Task<IEnumerable<ProjectSummaryDTO>> GetProjectSummariesFromResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<ProjectSummaryDTO>>(content, JsonOptions)!;
        }

        private static async Task<IEnumerable<UserDTO>> GetUsersFromResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<UserDTO>>(content, JsonOptions)!;
        }
    }
}