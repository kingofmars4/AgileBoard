using AgileBoard.Domain.Common;
using AgileBoard.Domain.Entities;

namespace AgileBoard.Services.Services.Interfaces
{
    public interface ITagService
    {
        Task<Result<IEnumerable<Tag>>> GetAllTagsAsync();
        Task<Result<Tag>> GetTagByIdAsync(int id);
        Task<Result<Tag>> GetTagByNameAsync(string name);
        Task<Result<Tag>> CreateTagAsync(string name);
        Task<Result<Tag>> UpdateTagAsync(int id, string? name);
        Task<Result<bool>> DeleteTagAsync(int id);
    }
}
