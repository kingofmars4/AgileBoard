using AgileBoard.API.DTOs;
using AgileBoard.Tests.Base;
using System.Net;
using System.Text.Json;

namespace AgileBoard.Tests.Sprints
{
    [TestFixture]
    public class SprintFunctionsTests : AuthenticatedTestBase
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
        public async Task CreateSprint_WithValidData_ShouldCreateSuccessfully()
        {
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = startDate.AddDays(14);
            
            var createSprintDto = new CreateSprintDTO("Sprint 1", "First sprint", _testProject.Id, startDate, endDate);
            var createResponse = await _client.PostAsJsonAsync("/api/sprint", createSprintDto);

            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var createdSprint = await GetSprintFromResponse(createResponse);
            Assert.Multiple(() =>
            {
                Assert.That(createdSprint.Name, Is.EqualTo("Sprint 1"));
                Assert.That(createdSprint.Description, Is.EqualTo("First sprint"));
                Assert.That(createdSprint.ProjectId, Is.EqualTo(_testProject.Id));
                Assert.That(createdSprint.DurationInDays, Is.EqualTo(15));
                Assert.That(createdSprint.Id, Is.GreaterThan(0));
            });
        }

        [Test]
        public async Task CreateSprint_WithEmptyName_ShouldReturnBadRequest()
        {
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = startDate.AddDays(14);
            
            var createSprintDto = new CreateSprintDTO("", "Description", _testProject.Id, startDate, endDate);
            var createResponse = await _client.PostAsJsonAsync("/api/sprint", createSprintDto);

            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task CreateSprint_WithNullName_ShouldReturnBadRequest()
        {
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = startDate.AddDays(14);
            
            var createSprintDto = new CreateSprintDTO(null!, "Description", _testProject.Id, startDate, endDate);
            var createResponse = await _client.PostAsJsonAsync("/api/sprint", createSprintDto);

            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task CreateSprint_WithEndDateBeforeStartDate_ShouldReturnBadRequest()
        {
            var startDate = DateTime.UtcNow.AddDays(10);
            var endDate = DateTime.UtcNow.AddDays(5);
            
            var createSprintDto = new CreateSprintDTO("Invalid Sprint", "Description", _testProject.Id, startDate, endDate);
            var createResponse = await _client.PostAsJsonAsync("/api/sprint", createSprintDto);

            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task CreateSprint_WithStartDateInPast_ShouldReturnBadRequest()
        {
            var startDate = DateTime.UtcNow.AddDays(-1);
            var endDate = DateTime.UtcNow.AddDays(14);
            
            var createSprintDto = new CreateSprintDTO("Past Sprint", "Description", _testProject.Id, startDate, endDate);
            var createResponse = await _client.PostAsJsonAsync("/api/sprint", createSprintDto);

            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task CreateSprint_WithDuplicateName_ShouldReturnConflict()
        {
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = startDate.AddDays(14);
            
            var createSprintDto1 = new CreateSprintDTO("Duplicate Sprint", "First", _testProject.Id, startDate, endDate);
            await _client.PostAsJsonAsync("/api/sprint", createSprintDto1);

            var createSprintDto2 = new CreateSprintDTO("Duplicate Sprint", "Second", _testProject.Id, startDate.AddDays(20), endDate.AddDays(20));
            var createResponse2 = await _client.PostAsJsonAsync("/api/sprint", createSprintDto2);

            Assert.That(createResponse2.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        }

        [Test]
        public async Task GetSprintById_WithValidId_ShouldReturnSprint()
        {
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = startDate.AddDays(14);
            
            var createSprintDto = new CreateSprintDTO("Get Test Sprint", "Description", _testProject.Id, startDate, endDate);
            var createResponse = await _client.PostAsJsonAsync("/api/sprint", createSprintDto);
            var createdSprint = await GetSprintFromResponse(createResponse);

            var getResponse = await _client.GetAsync($"/api/sprint/{createdSprint.Id}");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var retrievedSprint = await GetSprintFromResponse(getResponse);
            Assert.Multiple(() =>
            {
                Assert.That(retrievedSprint.Id, Is.EqualTo(createdSprint.Id));
                Assert.That(retrievedSprint.Name, Is.EqualTo("Get Test Sprint"));
                Assert.That(retrievedSprint.ProjectId, Is.EqualTo(_testProject.Id));
            });
        }

        [Test]
        public async Task GetSprintById_WithNonExistentId_ShouldReturnNotFound()
        {
            var getResponse = await _client.GetAsync("/api/sprint/99999");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GetSprintsByProject_ShouldReturnProjectSprints()
        {
            var startDate = DateTime.UtcNow.AddDays(1);
            
            await _client.PostAsJsonAsync("/api/sprint", new CreateSprintDTO("Sprint 1", "Desc 1", _testProject.Id, startDate, startDate.AddDays(14)));
            await _client.PostAsJsonAsync("/api/sprint", new CreateSprintDTO("Sprint 2", "Desc 2", _testProject.Id, startDate.AddDays(20), startDate.AddDays(34)));
            await _client.PostAsJsonAsync("/api/sprint", new CreateSprintDTO("Sprint 3", "Desc 3", _testProject.Id, startDate.AddDays(40), startDate.AddDays(54)));

            var getResponse = await _client.GetAsync($"/api/sprint/project/{_testProject.Id}");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var sprints = await GetSprintSummariesFromResponse(getResponse);
            Assert.That(sprints.Count(), Is.GreaterThanOrEqualTo(3));

            var sprintNames = sprints.Select(s => s.Name).ToList();
            Assert.Multiple(() =>
            {
                Assert.That(sprintNames, Does.Contain("Sprint 1"));
                Assert.That(sprintNames, Does.Contain("Sprint 2"));
                Assert.That(sprintNames, Does.Contain("Sprint 3"));
            });
        }

        [Test]
        public async Task GetSprintsByProject_WithNoSprints_ShouldReturnNotFound()
        {
            var newProjectDto = new CreateProjectDTO("Empty Project", "No sprints");
            var newProjectResponse = await _client.PostAsJsonAsync("/api/project", newProjectDto);
            var newProject = await GetProjectFromResponse(newProjectResponse);

            var getResponse = await _client.GetAsync($"/api/sprint/project/{newProject.Id}");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GetActiveSprints_ShouldReturnActiveSprints()
        {
            var now = DateTime.UtcNow;
            var startDate = now.AddHours(-1);
            var endDate = now.AddDays(5);

            await _client.PostAsJsonAsync("/api/sprint", new CreateSprintDTO("Active Sprint", "Currently running", _testProject.Id, startDate, endDate));

            var getResponse = await _client.GetAsync("/api/sprint/active");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var sprints = await GetSprintSummariesFromResponse(getResponse);
            var activeSprints = sprints.Where(s => s.IsActive).ToList();

            Assert.That(activeSprints.Any(s => s.Name == "Active Sprint"), Is.True);
        }

        [Test]
        public async Task UpdateSprint_WithValidData_ShouldUpdateSuccessfully()
        {
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = startDate.AddDays(14);
            
            var createSprintDto = new CreateSprintDTO("Original Sprint", "Original description", _testProject.Id, startDate, endDate);
            var createResponse = await _client.PostAsJsonAsync("/api/sprint", createSprintDto);
            var createdSprint = await GetSprintFromResponse(createResponse);

            var newStartDate = DateTime.UtcNow.AddDays(2);
            var newEndDate = newStartDate.AddDays(21);
            
            var updateSprintDto = new UpdateSprintDTO("Updated Sprint", "Updated description", newStartDate, newEndDate);
            var updateResponse = await _client.PutAsJsonAsync($"/api/sprint/{createdSprint.Id}", updateSprintDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var updatedSprint = await GetSprintFromResponse(updateResponse);
            Assert.Multiple(() =>
            {
                Assert.That(updatedSprint.Id, Is.EqualTo(createdSprint.Id));
                Assert.That(updatedSprint.Name, Is.EqualTo("Updated Sprint"));
                Assert.That(updatedSprint.Description, Is.EqualTo("Updated description"));
                Assert.That(updatedSprint.DurationInDays, Is.EqualTo(22));
            });
        }

        [Test]
        public async Task UpdateSprint_OnlyName_ShouldUpdateNameOnly()
        {
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = startDate.AddDays(14);
            
            var createSprintDto = new CreateSprintDTO("Name Update Test", "Original description", _testProject.Id, startDate, endDate);
            var createResponse = await _client.PostAsJsonAsync("/api/sprint", createSprintDto);
            var createdSprint = await GetSprintFromResponse(createResponse);

            var updateSprintDto = new UpdateSprintDTO("New Name Only", null, null, null);
            var updateResponse = await _client.PutAsJsonAsync($"/api/sprint/{createdSprint.Id}", updateSprintDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var updatedSprint = await GetSprintFromResponse(updateResponse);
            Assert.Multiple(() =>
            {
                Assert.That(updatedSprint.Name, Is.EqualTo("New Name Only"));
                Assert.That(updatedSprint.Description, Is.EqualTo("Original description"));
            });
        }

        [Test]
        public async Task UpdateSprint_WithEmptyData_ShouldReturnBadRequest()
        {
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = startDate.AddDays(14);
            
            var createSprintDto = new CreateSprintDTO("Empty Update Test", "Description", _testProject.Id, startDate, endDate);
            var createResponse = await _client.PostAsJsonAsync("/api/sprint", createSprintDto);
            var createdSprint = await GetSprintFromResponse(createResponse);

            var updateSprintDto = new UpdateSprintDTO(null, null, null, null);
            var updateResponse = await _client.PutAsJsonAsync($"/api/sprint/{createdSprint.Id}", updateSprintDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task UpdateSprint_WithNonExistentId_ShouldReturnNotFound()
        {
            var updateSprintDto = new UpdateSprintDTO("Updated Name", "Updated Description", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(15));
            var updateResponse = await _client.PutAsJsonAsync("/api/sprint/99999", updateSprintDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task DeleteSprint_WithValidId_ShouldDeleteSuccessfully()
        {
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = startDate.AddDays(14);
            
            var createSprintDto = new CreateSprintDTO("Sprint To Delete", "Will be deleted", _testProject.Id, startDate, endDate);
            var createResponse = await _client.PostAsJsonAsync("/api/sprint", createSprintDto);
            var createdSprint = await GetSprintFromResponse(createResponse);

            var deleteResponse = await _client.DeleteAsync($"/api/sprint/{createdSprint.Id}");

            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

            var getResponse = await _client.GetAsync($"/api/sprint/{createdSprint.Id}");
            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task DeleteSprint_WithNonExistentId_ShouldReturnNotFound()
        {
            var deleteResponse = await _client.DeleteAsync("/api/sprint/99999");

            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GetSprintsByDateRange_ShouldReturnSprintsInRange()
        {
            var baseDate = DateTime.UtcNow.AddDays(10);
            
            await _client.PostAsJsonAsync("/api/sprint", new CreateSprintDTO("Sprint In Range", "Description", _testProject.Id, baseDate, baseDate.AddDays(14)));
            await _client.PostAsJsonAsync("/api/sprint", new CreateSprintDTO("Sprint Outside Range", "Description", _testProject.Id, baseDate.AddDays(30), baseDate.AddDays(44)));

            var searchStart = baseDate.AddDays(-5);
            var searchEnd = baseDate.AddDays(20);
            
            var getResponse = await _client.GetAsync($"/api/sprint/date-range?startDate={searchStart:yyyy-MM-dd}&endDate={searchEnd:yyyy-MM-dd}");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var sprints = await GetSprintSummariesFromResponse(getResponse);
            var sprintNames = sprints.Select(s => s.Name).ToList();
            
            Assert.Multiple(() =>
            {
                Assert.That(sprintNames, Does.Contain("Sprint In Range"));
                Assert.That(sprintNames, Does.Not.Contain("Sprint Outside Range"));
            });
        }

        private static async Task<SprintDTO> GetSprintFromResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SprintDTO>(content, JsonOptions)!;
        }

        private static async Task<IEnumerable<SprintSummaryDTO>> GetSprintSummariesFromResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<SprintSummaryDTO>>(content, JsonOptions)!;
        }

        private static async Task<ProjectDTO> GetProjectFromResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProjectDTO>(content, JsonOptions)!;
        }
    }
}