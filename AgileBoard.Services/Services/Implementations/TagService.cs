using AgileBoard.Domain.Common;
using AgileBoard.Domain.Constants;
using AgileBoard.Domain.Entities;
using AgileBoard.Infrastructure.Repositories.Interfaces;
using AgileBoard.Services.Services.Interfaces;

namespace AgileBoard.Services.Services.Implementations
{
    public class TagService(ITagRepository tagRepository) 
        : ITagService
    {
        private readonly ITagRepository _tagRepository = tagRepository;

        public async Task<Result<Tag>> GetTagByIdAsync(int id)
        {
            var tag = await _tagRepository.GetTagByIdAsync(id);
            if (tag == null)
                return Result<Tag>.NotFound(Messages.EntityNames.Tag);

            return Result<Tag>.Success(tag);
        }

        public async Task<Result<Tag>> GetTagByNameAsync(string name)
        {
            var tag = await _tagRepository.GetTagByNameAsync(name);
            if (tag == null)
                return Result<Tag>.NotFound(Messages.EntityNames.Tag);

            return Result<Tag>.Success(tag);
        }

        public async Task<Result<IEnumerable<Tag>>> GetAllTagsAsync()
        {
            var tags = await _tagRepository.GetAllTagsAsync();

            if (!tags.Any())
                return Result<IEnumerable<Tag>>.NotFound(Messages.EntityNames.Tags, isPlural: true);

            return Result<IEnumerable<Tag>>.Success(tags);
        }

        public async Task<Result<Tag>> CreateTagAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                return Result<Tag>.BadRequest(Messages.Tags.TagNameRequired);

            var existingTag = await _tagRepository.GetTagByNameAsync(name);
            if (existingTag != null)
                return Result<Tag>.Conflict(Messages.Registration.TagnameAlreadyExists);

            try
            {
                var newTag = new Tag { Name = name };
                var createdTag = await _tagRepository.AddTagAsync(newTag);
                return Result<Tag>.Success(createdTag);
            }
            catch (Exception) 
            { 
                return Result<Tag>.Failure(Messages.Generic.InternalServerError); 
            }
        }

        public async Task<Result<Tag>> UpdateTagAsync(int id, string? name)
        {
            if (string.IsNullOrEmpty(name))
                return Result<Tag>.BadRequest(Messages.TagUpdate.NoNameSpecified);

            try
            {
                var updatedTag = await _tagRepository.UpdateTagAsync(id, name);
                if (updatedTag == null)
                    return Result<Tag>.NotFound(Messages.EntityNames.Tag);

                return Result<Tag>.Success(updatedTag);
            }
            catch (Exception) 
            { 
                return Result<Tag>.Failure(Messages.Generic.InternalServerError); 
            }
        }

        public async Task<Result<bool>> DeleteTagAsync(int id)
        {
            var tag = await _tagRepository.GetTagByIdAsync(id);
            if (tag == null)
                return Result<bool>.NotFound(Messages.EntityNames.Tag);

            try
            {
                var deleted = await _tagRepository.DeleteTagAsync(id);
                return Result<bool>.Success(deleted);
            }
            catch (Exception)
            {
                return Result<bool>.Failure(Messages.Generic.InternalServerError);
            }
        }
    }
}
