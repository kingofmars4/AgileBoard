using AgileBoard.API.DTOs;
using AgileBoard.Tests.Base;
using System.Net;
using System.Text.Json;

namespace AgileBoard.Tests.Tags
{
    [TestFixture]
    public class TagFunctionsTests : AuthenticatedTestBase
    {
        [Test]
        public async Task CreateTag_WithValidData_ShouldCreateSuccessfully()
        {
            var createTagDto = new CreateTagDTO("Bug");
            var createResponse = await _client.PostAsJsonAsync("/api/tag", createTagDto);

            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var createdTag = await GetTagFromResponse(createResponse);
            Assert.Multiple(() =>
            {
                Assert.That(createdTag.Name, Is.EqualTo("Bug"));
                Assert.That(createdTag.Id, Is.GreaterThan(0));
            });
        }

        [Test]
        public async Task CreateTag_WithEmptyName_ShouldReturnBadRequest()
        {
            var createTagDto = new CreateTagDTO("");
            var createResponse = await _client.PostAsJsonAsync("/api/tag", createTagDto);

            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task CreateTag_WithNullName_ShouldReturnBadRequest()
        {
            var createTagDto = new CreateTagDTO(null!);
            var createResponse = await _client.PostAsJsonAsync("/api/tag", createTagDto);

            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task CreateTag_WithDuplicateName_ShouldReturnConflict()
        {
            var createTagDto1 = new CreateTagDTO("Feature");
            await _client.PostAsJsonAsync("/api/tag", createTagDto1);

            var createTagDto2 = new CreateTagDTO("Feature");
            var createResponse2 = await _client.PostAsJsonAsync("/api/tag", createTagDto2);

            Assert.That(createResponse2.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        }

        [Test]
        public async Task GetTagById_WithValidId_ShouldReturnTag()
        {
            var createTagDto = new CreateTagDTO("Enhancement");
            var createResponse = await _client.PostAsJsonAsync("/api/tag", createTagDto);
            var createdTag = await GetTagFromResponse(createResponse);

            var getResponse = await _client.GetAsync($"/api/tag/{createdTag.Id}");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            
            var retrievedTag = await GetTagFromResponse(getResponse);
            Assert.Multiple(() =>
            {
                Assert.That(retrievedTag.Id, Is.EqualTo(createdTag.Id));
                Assert.That(retrievedTag.Name, Is.EqualTo("Enhancement"));
            });
        }

        [Test]
        public async Task GetTagById_WithNonExistentId_ShouldReturnNotFound()
        {
            var getResponse = await _client.GetAsync("/api/tag/99999");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GetTagByName_WithValidName_ShouldReturnTag()
        {
            var createTagDto = new CreateTagDTO("Documentation");
            var createResponse = await _client.PostAsJsonAsync("/api/tag", createTagDto);
            var createdTag = await GetTagFromResponse(createResponse);

            var getResponse = await _client.GetAsync("/api/tag/by-name/Documentation");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            
            var retrievedTag = await GetTagFromResponse(getResponse);
            Assert.Multiple(() =>
            {
                Assert.That(retrievedTag.Id, Is.EqualTo(createdTag.Id));
                Assert.That(retrievedTag.Name, Is.EqualTo("Documentation"));
            });
        }

        [Test]
        public async Task GetTagByName_WithNonExistentName_ShouldReturnNotFound()
        {
            var getResponse = await _client.GetAsync("/api/tag/by-name/NonExistentTag");

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GetAllTags_WithExistingTags_ShouldReturnAllTags()
        {
            await _client.PostAsJsonAsync("/api/tag", new CreateTagDTO("Bug"));
            await _client.PostAsJsonAsync("/api/tag", new CreateTagDTO("Feature"));
            await _client.PostAsJsonAsync("/api/tag", new CreateTagDTO("Enhancement"));

            var getAllResponse = await _client.GetAsync("/api/tag");

            Assert.That(getAllResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            
            var tags = await GetTagsFromResponse(getAllResponse);
            Assert.That(tags.Count(), Is.GreaterThanOrEqualTo(3));
            
            var tagNames = tags.Select(t => t.Name).ToList();
            Assert.Multiple(() =>
            {
                Assert.That(tagNames, Does.Contain("Bug"));
                Assert.That(tagNames, Does.Contain("Feature"));
                Assert.That(tagNames, Does.Contain("Enhancement"));
            });
        }

        [Test]
        public async Task GetAllTags_WithNoTags_ShouldReturnNotFound()
        {
            var getAllResponse = await _client.GetAsync("/api/tag");

            Assert.That(getAllResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task UpdateTag_WithValidData_ShouldUpdateSuccessfully()
        {
            var createTagDto = new CreateTagDTO("OriginalTag");
            var createResponse = await _client.PostAsJsonAsync("/api/tag", createTagDto);
            var createdTag = await GetTagFromResponse(createResponse);

            var updateTagDto = new UpdateTagDTO("UpdatedTag");
            var updateResponse = await _client.PutAsJsonAsync($"/api/tag/{createdTag.Id}", updateTagDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            
            var updatedTag = await GetTagFromResponse(updateResponse);
            Assert.Multiple(() =>
            {
                Assert.That(updatedTag.Id, Is.EqualTo(createdTag.Id));
                Assert.That(updatedTag.Name, Is.EqualTo("UpdatedTag"));
            });
        }

        [Test]
        public async Task UpdateTag_WithEmptyName_ShouldReturnBadRequest()
        {
            var createTagDto = new CreateTagDTO("TestTag");
            var createResponse = await _client.PostAsJsonAsync("/api/tag", createTagDto);
            var createdTag = await GetTagFromResponse(createResponse);

            var updateTagDto = new UpdateTagDTO("");
            var updateResponse = await _client.PutAsJsonAsync($"/api/tag/{createdTag.Id}", updateTagDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task UpdateTag_WithNonExistentId_ShouldReturnNotFound()
        {
            var updateTagDto = new UpdateTagDTO("UpdatedTag");
            var updateResponse = await _client.PutAsJsonAsync("/api/tag/99999", updateTagDto);

            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task DeleteTag_WithValidId_ShouldDeleteSuccessfully()
        {
            var createTagDto = new CreateTagDTO("TagToDelete");
            var createResponse = await _client.PostAsJsonAsync("/api/tag", createTagDto);
            var createdTag = await GetTagFromResponse(createResponse);

            var deleteResponse = await _client.DeleteAsync($"/api/tag/{createdTag.Id}");

            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var getResponse = await _client.GetAsync($"/api/tag/{createdTag.Id}");
            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task DeleteTag_WithNonExistentId_ShouldReturnNotFound()
        {
            var deleteResponse = await _client.DeleteAsync("/api/tag/99999");

            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
        private static async Task<TagDTO> GetTagFromResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TagDTO>(content, JsonOptions)!;
        }

        private static async Task<IEnumerable<TagDTO>> GetTagsFromResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<TagDTO>>(content, JsonOptions)!;
        }
    }
}
