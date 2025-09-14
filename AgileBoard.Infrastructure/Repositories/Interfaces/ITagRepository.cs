using AgileBoard.Domain.Entities;

namespace AgileBoard.Infrastructure.Repositories.Interfaces
{
    public interface ITagRepository
    {
        Task<Tag> AddTagAsync(Tag newTag);
        Task<Tag?> GetTagByIdAsync(int id);
        Task<Tag?> GetTagByNameAsync(string name);
        Task<IEnumerable<Tag>> GetAllTagsAsync();
        Task<Tag> UpdateTagAsync(int id, string? Name);
        Task<bool> DeleteTagAsync(int id);
    }
}
