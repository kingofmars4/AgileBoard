using AgileBoard.API.DTOs;
using AgileBoard.Tests.Base;
using System.Net;
using System.Text.Json;
using static AgileBoard.Domain.Entities.WorkItem;

namespace AgileBoard.Tests.WorkItems
{
    [TestFixture]
    public class WorkItemFunctionsTests : AuthenticatedTestBase
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
        public async Task CreateWorkItem_WithValidData_ShouldCreateSuccessfully()
        {
            var createWorkItemDto = new CreateWorkItemDTO("Test Work Item", "A test work item", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);

            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var createdWorkItem = await GetWorkItemFromResponse(createResponse);
            Assert.Multiple(() =>
            {
                Assert.That(createdWorkItem.Name, Is.EqualTo("Test Work Item"));
                Assert.That(createdWorkItem.Description, Is.EqualTo("A test work item"));
                Assert.That(createdWorkItem.ProjectId, Is.EqualTo(_testProject.Id));
                Assert.That(createdWorkItem.State, Is.EqualTo(WorkItemState.ToDo));
                Assert.That(createdWorkItem.Id, Is.GreaterThan(0));
            });
        }

        [Test]
        public async Task CreateWorkItem_WithEmptyName_ShouldReturnBadRequest()
        {
            var createWorkItemDto = new CreateWorkItemDTO("", "Description", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);

            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task CreateWorkItem_WithNullName_ShouldReturnBadRequest()
        {
            var createWorkItemDto = new CreateWorkItemDTO(null!, "Description", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);

            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task CreateWorkItem_WithInvalidProjectId_ShouldReturnBadRequest()
        {
            var createWorkItemDto = new CreateWorkItemDTO("Test Item", "Description", 0);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);

            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task CreateWorkItem_WithDifferentStates_ShouldCreateSuccessfully()
        {
            var states = new[] { WorkItemState.ToDo, WorkItemState.Doing, WorkItemState.Done };

            foreach (var state in states)
            {
                var createWorkItemDto = new CreateWorkItemDTO($"Item {state}", "Description", _testProject.Id, state);
                var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);

                Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

                var createdWorkItem = await GetWorkItemFromResponse(createResponse);
                Assert.That(createdWorkItem.State, Is.EqualTo(state));
            }
        }

        [Test]
        public async Task GetWorkItemById_WithValidId_ShouldReturnWorkItem()
        {
            var createWorkItemDto = new CreateWorkItemDTO("Get Test Item", "Description", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);
            var createdWorkItem = await GetWorkItemFromResponse(createResponse);

            var getResponse = await _client.GetAsync($"/api/workitem/{createdWorkItem.Id}");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var retrievedWorkItem = await GetWorkItemFromResponse(getResponse);
            Assert.Multiple(() =>
            {
                Assert.That(retrievedWorkItem.Id, Is.EqualTo(createdWorkItem.Id));
                Assert.That(retrievedWorkItem.Name, Is.EqualTo("Get Test Item"));
                Assert.That(retrievedWorkItem.ProjectId, Is.EqualTo(_testProject.Id));
            });
        }

        [Test]
        public async Task GetWorkItemById_WithNonExistentId_ShouldReturnNotFound()
        {
            var getResponse = await _client.GetAsync("/api/workitem/99999");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GetWorkItemsByProject_ShouldReturnProjectWorkItems()
        {
            await _client.PostAsJsonAsync("/api/workitem", new CreateWorkItemDTO("Item 1", "Desc 1", _testProject.Id));
            await _client.PostAsJsonAsync("/api/workitem", new CreateWorkItemDTO("Item 2", "Desc 2", _testProject.Id));
            await _client.PostAsJsonAsync("/api/workitem", new CreateWorkItemDTO("Item 3", "Desc 3", _testProject.Id));

            var getResponse = await _client.GetAsync($"/api/workitem/project/{_testProject.Id}");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var workItems = await GetWorkItemSummariesFromResponse(getResponse);
            Assert.That(workItems.Count(), Is.GreaterThanOrEqualTo(3));

            var itemNames = workItems.Select(w => w.Name).ToList();
            Assert.Multiple(() =>
            {
                Assert.That(itemNames, Does.Contain("Item 1"));
                Assert.That(itemNames, Does.Contain("Item 2"));
                Assert.That(itemNames, Does.Contain("Item 3"));
            });
        }

        [Test]
        public async Task GetWorkItemsByProject_WithNoWorkItems_ShouldReturnNotFound()
        {
            var newProjectDto = new CreateProjectDTO("Empty Project", "No work items");
            var newProjectResponse = await _client.PostAsJsonAsync("/api/project", newProjectDto);
            var newProject = await GetProjectFromResponse(newProjectResponse);

            var getResponse = await _client.GetAsync($"/api/workitem/project/{newProject.Id}");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GetWorkItemsByState_ShouldReturnCorrectItems()
        {
            await _client.PostAsJsonAsync("/api/workitem", new CreateWorkItemDTO("ToDo Item", "Desc", _testProject.Id, WorkItemState.ToDo));
            await _client.PostAsJsonAsync("/api/workitem", new CreateWorkItemDTO("Doing Item", "Desc", _testProject.Id, WorkItemState.Doing));
            await _client.PostAsJsonAsync("/api/workitem", new CreateWorkItemDTO("Done Item", "Desc", _testProject.Id, WorkItemState.Done));

            var getToDoResponse = await _client.GetAsync("/api/workitem/state/ToDo");
            var getDoingResponse = await _client.GetAsync("/api/workitem/state/Doing");
            var getDoneResponse = await _client.GetAsync("/api/workitem/state/Done");

            Assert.Multiple(() =>
            {
                Assert.That(getToDoResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(getDoingResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(getDoneResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            });

            var toDoItems = await GetWorkItemSummariesFromResponse(getToDoResponse);
            var doingItems = await GetWorkItemSummariesFromResponse(getDoingResponse);
            var doneItems = await GetWorkItemSummariesFromResponse(getDoneResponse);

            Assert.Multiple(() =>
            {
                Assert.That(toDoItems.Any(w => w.Name == "ToDo Item"), Is.True);
                Assert.That(doingItems.Any(w => w.Name == "Doing Item"), Is.True);
                Assert.That(doneItems.Any(w => w.Name == "Done Item"), Is.True);
            });
        }

        [Test]
        public async Task UpdateWorkItem_WithValidData_ShouldUpdateSuccessfully()
        {
            var createWorkItemDto = new CreateWorkItemDTO("Original Item", "Original description", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);
            var createdWorkItem = await GetWorkItemFromResponse(createResponse);

            var updateWorkItemDto = new UpdateWorkItemDTO("Updated Item", "Updated description", WorkItemState.Doing, 5, null);
            var updateResponse = await _client.PutAsJsonAsync($"/api/workitem/{createdWorkItem.Id}", updateWorkItemDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var updatedWorkItem = await GetWorkItemFromResponse(updateResponse);
            Assert.Multiple(() =>
            {
                Assert.That(updatedWorkItem.Id, Is.EqualTo(createdWorkItem.Id));
                Assert.That(updatedWorkItem.Name, Is.EqualTo("Updated Item"));
                Assert.That(updatedWorkItem.Description, Is.EqualTo("Updated description"));
                Assert.That(updatedWorkItem.State, Is.EqualTo(WorkItemState.Doing));
                Assert.That(updatedWorkItem.Index, Is.EqualTo(5));
            });
        }

        [Test]
        public async Task UpdateWorkItem_OnlyName_ShouldUpdateNameOnly()
        {
            var createWorkItemDto = new CreateWorkItemDTO("Name Update Test", "Original description", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);
            var createdWorkItem = await GetWorkItemFromResponse(createResponse);

            var updateWorkItemDto = new UpdateWorkItemDTO("New Name Only", null, null, null, null);
            var updateResponse = await _client.PutAsJsonAsync($"/api/workitem/{createdWorkItem.Id}", updateWorkItemDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var updatedWorkItem = await GetWorkItemFromResponse(updateResponse);
            Assert.Multiple(() =>
            {
                Assert.That(updatedWorkItem.Name, Is.EqualTo("New Name Only"));
                Assert.That(updatedWorkItem.Description, Is.EqualTo("Original description"));
                Assert.That(updatedWorkItem.State, Is.EqualTo(WorkItemState.ToDo));
            });
        }

        [Test]
        public async Task UpdateWorkItem_WithEmptyData_ShouldReturnBadRequest()
        {
            var createWorkItemDto = new CreateWorkItemDTO("Empty Update Test", "Description", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);
            var createdWorkItem = await GetWorkItemFromResponse(createResponse);

            var updateWorkItemDto = new UpdateWorkItemDTO(null, null, null, null, null);
            var updateResponse = await _client.PutAsJsonAsync($"/api/workitem/{createdWorkItem.Id}", updateWorkItemDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task UpdateWorkItem_WithNonExistentId_ShouldReturnNotFound()
        {
            var updateWorkItemDto = new UpdateWorkItemDTO("Updated Name", "Updated Description", WorkItemState.Done, 1, null);
            var updateResponse = await _client.PutAsJsonAsync("/api/workitem/99999", updateWorkItemDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task DeleteWorkItem_WithValidId_ShouldDeleteSuccessfully()
        {
            var createWorkItemDto = new CreateWorkItemDTO("Item To Delete", "Will be deleted", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);
            var createdWorkItem = await GetWorkItemFromResponse(createResponse);

            var deleteResponse = await _client.DeleteAsync($"/api/workitem/{createdWorkItem.Id}");

            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

            var getResponse = await _client.GetAsync($"/api/workitem/{createdWorkItem.Id}");
            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task DeleteWorkItem_WithNonExistentId_ShouldReturnNotFound()
        {
            var deleteResponse = await _client.DeleteAsync("/api/workitem/99999");

            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task AssignUser_ShouldAssignSuccessfully()
        {
            var createWorkItemDto = new CreateWorkItemDTO("Assignment Test", "For testing user assignment", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);
            var createdWorkItem = await GetWorkItemFromResponse(createResponse);

            var assignUserDto = new AssignUserDTO(_currentUser.Id);
            var assignResponse = await _client.PostAsJsonAsync($"/api/workitem/{createdWorkItem.Id}/assign", assignUserDto);

            Assert.That(assignResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task UnassignUser_ShouldUnassignSuccessfully()
        {
            var createWorkItemDto = new CreateWorkItemDTO("Unassignment Test", "For testing user unassignment", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);
            var createdWorkItem = await GetWorkItemFromResponse(createResponse);

            await _client.PostAsJsonAsync($"/api/workitem/{createdWorkItem.Id}/assign", new AssignUserDTO(_currentUser.Id));

            var unassignResponse = await _client.DeleteAsync($"/api/workitem/{createdWorkItem.Id}/unassign/{_currentUser.Id}");

            Assert.That(unassignResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task AddTag_ShouldAddSuccessfully()
        {
            var createWorkItemDto = new CreateWorkItemDTO("Tag Test", "For testing tag addition", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);
            var createdWorkItem = await GetWorkItemFromResponse(createResponse);

            var createTagResponse = await _client.PostAsJsonAsync("/api/tag", new CreateTagDTO("Test Tag"));
            var createdTag = await GetTagFromResponse(createTagResponse);

            var addTagDto = new AddTagToWorkItemDTO(createdTag.Id);
            var addTagResponse = await _client.PostAsJsonAsync($"/api/workitem/{createdWorkItem.Id}/tags", addTagDto);

            Assert.That(addTagResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task RemoveTag_ShouldRemoveSuccessfully()
        {
            var createWorkItemDto = new CreateWorkItemDTO("Tag Remove Test", "For testing tag removal", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);
            var createdWorkItem = await GetWorkItemFromResponse(createResponse);

            var createTagResponse = await _client.PostAsJsonAsync("/api/tag", new CreateTagDTO("Remove Test Tag"));
            var createdTag = await GetTagFromResponse(createTagResponse);

            await _client.PostAsJsonAsync($"/api/workitem/{createdWorkItem.Id}/tags", new AddTagToWorkItemDTO(createdTag.Id));

            var removeTagResponse = await _client.DeleteAsync($"/api/workitem/{createdWorkItem.Id}/tags/{createdTag.Id}");

            Assert.That(removeTagResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task MoveToSprint_ShouldMoveSuccessfully()
        {
            var createWorkItemDto = new CreateWorkItemDTO("Sprint Move Test", "For testing sprint movement", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);
            var createdWorkItem = await GetWorkItemFromResponse(createResponse);

            var moveToSprintDto = new MoveToSprintDTO(null);
            var moveResponse = await _client.PutAsJsonAsync($"/api/workitem/{createdWorkItem.Id}/sprint", moveToSprintDto);

            Assert.That(moveResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task UpdateIndex_ShouldUpdateSuccessfully()
        {
            var createWorkItemDto = new CreateWorkItemDTO("Index Update Test", "For testing index update", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);
            var createdWorkItem = await GetWorkItemFromResponse(createResponse);

            var updateIndexDto = new UpdateIndexDTO(10);
            var updateResponse = await _client.PutAsJsonAsync($"/api/workitem/{createdWorkItem.Id}/index", updateIndexDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task UpdateIndex_WithNegativeIndex_ShouldReturnBadRequest()
        {
            var createWorkItemDto = new CreateWorkItemDTO("Negative Index Test", "For testing negative index", _testProject.Id);
            var createResponse = await _client.PostAsJsonAsync("/api/workitem", createWorkItemDto);
            var createdWorkItem = await GetWorkItemFromResponse(createResponse);

            var updateIndexDto = new UpdateIndexDTO(-1);
            var updateResponse = await _client.PutAsJsonAsync($"/api/workitem/{createdWorkItem.Id}/index", updateIndexDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        private static async Task<WorkItemDTO> GetWorkItemFromResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<WorkItemDTO>(content, JsonOptions)!;
        }

        private static async Task<IEnumerable<WorkItemSummaryDTO>> GetWorkItemSummariesFromResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<WorkItemSummaryDTO>>(content, JsonOptions)!;
        }

        private static async Task<ProjectDTO> GetProjectFromResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProjectDTO>(content, JsonOptions)!;
        }

        private static async Task<TagDTO> GetTagFromResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TagDTO>(content, JsonOptions)!;
        }
    }
}