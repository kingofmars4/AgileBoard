using AgileBoard.API.DTOs;
using AgileBoard.Tests.Base;
using System.Net;
using static AgileBoard.Domain.Entities.WorkItem;

namespace AgileBoard.Tests.WorkItems
{
    [TestFixture]
    public class WorkItemUnauthorizedTests : AuthenticatedTestBase
    {
        private ProjectDTO _testProject;

        [SetUp]
        public override async Task SetUp()
        {
            await base.SetUp();
            
            var createProjectDto = new CreateProjectDTO("Test Project", "Project for work item tests");
            var projectResponse = await _client.PostAsJsonAsync("/api/project", createProjectDto);
            _testProject = await GetProjectFromResponse(projectResponse);
        }

        [Test]
        public async Task CreateWorkItem_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            ClearAuthentication();

            var createWorkItemDto = new CreateWorkItemDTO("Test Item", "Description", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);

            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GetAllWorkItems_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            ClearAuthentication();

            var getResponse = await _client.GetAsync("/api/workitem");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GetWorkItemById_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            ClearAuthentication();

            var getResponse = await _client.GetAsync("/api/workitem/1");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task UpdateWorkItem_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            ClearAuthentication();

            var updateWorkItemDto = new UpdateWorkItemDTO("Updated Name", "Updated Description", WorkItemState.Done, 1, null);
            var updateResponse = await _client.PutAsJsonAsync("/api/workitem/1", updateWorkItemDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task DeleteWorkItem_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            ClearAuthentication();

            var deleteResponse = await _client.DeleteAsync("/api/workitem/1");

            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task CreateWorkItem_InProjectUserDoesntOwn_ShouldReturnForbidden()
        {
            await CreateAdditionalUserAsync("user2", "user2@example.com", "Password123!");
            await SwitchToUserAsync("user2", "Password123!");

            var createWorkItemDto = new CreateWorkItemDTO("Unauthorized Item", "Should not create", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);

            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task GetWorkItemById_FromProjectUserCantAccess_ShouldReturnForbidden()
        {
            var createWorkItemDto = new CreateWorkItemDTO("Private Item", "Only for owner", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);
            var createdWorkItem = await GetWorkItemFromResponse(createResponse);

            await CreateAdditionalUserAsync("user2", "user2@example.com", "Password123!");
            await SwitchToUserAsync("user2", "Password123!");

            var getResponse = await _client.GetAsync($"/api/workitem/{createdWorkItem.Id}");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task UpdateWorkItem_AsNonProjectMember_ShouldReturnForbidden()
        {
            var createWorkItemDto = new CreateWorkItemDTO("Update Test Item", "Description", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);
            var createdWorkItem = await GetWorkItemFromResponse(createResponse);

            await CreateAdditionalUserAsync("user2", "user2@example.com", "Password123!");
            await SwitchToUserAsync("user2", "Password123!");

            var updateWorkItemDto = new UpdateWorkItemDTO("Hacked Name", "Hacked Description", WorkItemState.Done, 1, null);
            var updateResponse = await _client.PutAsJsonAsync($"/api/workitem/{createdWorkItem.Id}", updateWorkItemDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task DeleteWorkItem_AsNonProjectMember_ShouldReturnForbidden()
        {
            var createWorkItemDto = new CreateWorkItemDTO("Delete Test Item", "Description", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);
            var createdWorkItem = await GetWorkItemFromResponse(createResponse);

            await CreateAdditionalUserAsync("user2", "user2@example.com", "Password123!");
            await SwitchToUserAsync("user2", "Password123!");

            var deleteResponse = await _client.DeleteAsync($"/api/workitem/{createdWorkItem.Id}");

            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task GetWorkItemById_AsProjectParticipant_ShouldReturnWorkItem()
        {
            var createWorkItemDto = new CreateWorkItemDTO("Participant Access Test", "Description", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);
            var createdWorkItem = await GetWorkItemFromResponse(createResponse);

            var user2 = await CreateAdditionalUserAsync("participant", "participant@example.com", "Password123!");

            await _client.PostAsJsonAsync($"/api/project/{_testProject.Id}/participants", new AddParticipantDTO(user2.Id));

            await SwitchToUserAsync("participant", "Password123!");

            var getResponse = await _client.GetAsync($"/api/workitem/{createdWorkItem.Id}");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var workItem = await GetWorkItemFromResponse(getResponse);
            Assert.That(workItem.Id, Is.EqualTo(createdWorkItem.Id));
        }

        [Test]
        public async Task UpdateWorkItem_AsProjectParticipant_ShouldUpdateSuccessfully()
        {
            var createWorkItemDto = new CreateWorkItemDTO("Participant Update Test", "Description", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);
            var createdWorkItem = await GetWorkItemFromResponse(createResponse);

            var user2 = await CreateAdditionalUserAsync("participant", "participant@example.com", "Password123!");

            await _client.PostAsJsonAsync($"/api/project/{_testProject.Id}/participants", new AddParticipantDTO(user2.Id));

            await SwitchToUserAsync("participant", "Password123!");

            var updateWorkItemDto = new UpdateWorkItemDTO("Updated by Participant", "Updated description", WorkItemState.Doing, 1, null);
            var updateResponse = await _client.PutAsJsonAsync($"/api/workitem/{createdWorkItem.Id}", updateWorkItemDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var updatedWorkItem = await GetWorkItemFromResponse(updateResponse);
            Assert.That(updatedWorkItem.Name, Is.EqualTo("Updated by Participant"));
        }

        [Test]
        public async Task AssignUser_AsNonProjectMember_ShouldReturnForbidden()
        {
            var createWorkItemDto = new CreateWorkItemDTO("Assignment Test", "Description", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);
            var createdWorkItem = await GetWorkItemFromResponse(createResponse);

            await CreateAdditionalUserAsync("user2", "user2@example.com", "Password123!");
            await SwitchToUserAsync("user2", "Password123!");

            var assignUserDto = new AssignUserDTO(_currentUser.Id);
            var assignResponse = await _client.PostAsJsonAsync($"/api/workitem/{createdWorkItem.Id}/assign", assignUserDto);

            Assert.That(assignResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task AddTag_AsNonProjectMember_ShouldReturnForbidden()
        {
            var createWorkItemDto = new CreateWorkItemDTO("Tag Test", "Description", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);
            var createdWorkItem = await GetWorkItemFromResponse(createResponse);

            var createTagResponse = await _client.PostAsJsonAsync("/api/tag", new CreateTagDTO("Test Tag"));
            var createdTag = await GetTagFromResponse(createTagResponse);

            await CreateAdditionalUserAsync("user2", "user2@example.com", "Password123!");
            await SwitchToUserAsync("user2", "Password123!");

            var addTagDto = new AddTagToWorkItemDTO(createdTag.Id);
            var addTagResponse = await _client.PostAsJsonAsync($"/api/workitem/{createdWorkItem.Id}/tags", addTagDto);

            Assert.That(addTagResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task GetWorkItemsByProject_AsNonProjectMember_ShouldReturnForbidden()
        {
            await CreateAdditionalUserAsync("user2", "user2@example.com", "Password123!");
            await SwitchToUserAsync("user2", "Password123!");

            var getResponse = await _client.GetAsync($"/api/workitem/project/{_testProject.Id}");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task CompleteWorkItemFlow_WithMultipleUsers()
        {
            var createWorkItemDto = new CreateWorkItemDTO("Multi-User Test", "Testing access levels", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);
            var createdWorkItem = await GetWorkItemFromResponse(createResponse);

            var user2 = await CreateAdditionalUserAsync("user2", "user2@example.com", "Password123!");
            await CreateAdditionalUserAsync("user3", "user3@example.com", "Password123!");

            await _client.PostAsJsonAsync($"/api/project/{_testProject.Id}/participants", new AddParticipantDTO(user2.Id));

            await SwitchToUserAsync("user2", "Password123!");
            var getResponse2 = await _client.GetAsync($"/api/workitem/{createdWorkItem.Id}");
            Assert.That(getResponse2.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            await SwitchToUserAsync("user3", "Password123!");
            var getResponse3 = await _client.GetAsync($"/api/workitem/{createdWorkItem.Id}");
            Assert.That(getResponse3.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

            await SwitchToUserAsync("user2", "Password123!");
            var updateResponse = await _client.PutAsJsonAsync($"/api/workitem/{createdWorkItem.Id}", 
                new UpdateWorkItemDTO("Updated by participant", null, WorkItemState.Doing, null, null));
            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        private static async Task<WorkItemDTO> GetWorkItemFromResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<WorkItemDTO>(content, JsonOptions)!;
        }

        private static async Task<ProjectDTO> GetProjectFromResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<ProjectDTO>(content, JsonOptions)!;
        }

        private static async Task<TagDTO> GetTagFromResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<TagDTO>(content, JsonOptions)!;
        }
    }
}