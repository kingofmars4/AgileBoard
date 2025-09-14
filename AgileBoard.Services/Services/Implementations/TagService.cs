using AgileBoard.Domain.Common;
using AgileBoard.Domain.Constants;
using AgileBoard.Domain.Entities;
using AgileBoard.Infrastructure.Repositories.Interfaces;
using AgileBoard.Services.Security.Interfaces;
using AgileBoard.Services.Services.Interfaces;

namespace AgileBoard.Services.Services.Implementations
{
    public class TagService(ITagRepository tagRepository, IPasswordHasher passwordHasher, IJwtService jwtService) 
        : ITagService
    {
        private readonly ITagRepository _tagRepository = tagRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IJwtService _jwtService = jwtService;

        public async Task<Result<Tag>> GetTagByIdAsync(int id)
        {
            var Tag = await _tagRepository.GetTagByIdAsync(id);
            if (Tag == null)
                return Result<Tag>.NotFound(Messages.TagRetrieval.TagNotFound);

            return Result<Tag>.Success(Tag);
        }

        public async Task<Result<Tag>> GetTagByNameAsync(string name)
        {
            var Tag = await _tagRepository.GetTagByNameAsync(name);
            if (Tag == null)
                return Result<Tag>.NotFound(Messages.TagRetrieval.TagNotFound);

            return Result<Tag>.Success(Tag);
        }

        public async Task<Result<IEnumerable<Tag>>> GetAllTagsAsync()
        {
            var Tags = await _tagRepository.GetAllTagsAsync();
            if (!Tags.Any())
                return Result<IEnumerable<Tag>>.NotFound(Messages.TagRetrieval.TagsNotFound);

            return Result<IEnumerable<Tag>>.Success(Tags);
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
                var newTag = new Tag
                {
                    Name = name
                };

                var createdTag = await _tagRepository.AddTagAsync(newTag);
                return Result<Tag>.Success(createdTag);
            }
            catch (Exception) { return Result<Tag>.Failure(Messages.Generic.InternalServerError); }
        }

        public async Task<Result<Tag>> UpdateTagAsync(int id, string? name)
        {
            if (string.IsNullOrEmpty(name))
                return Result<Tag>.BadRequest(Messages.TagUpdate.NoNameSpecified);

            try
            {
                var updatedTag = await _tagRepository.UpdateTagAsync(id, name);
                if (updatedTag == null)
                    return Result<Tag>.NotFound(Messages.TagRetrieval.TagNotFound);

                return Result<Tag>.Success(updatedTag);
            }
            catch (Exception) { return Result<Tag>.Failure(Messages.Generic.InternalServerError); }
        }

        public async Task<Result<bool>> DeleteTagAsync(int id)
        {
            var tag = await _tagRepository.GetTagByIdAsync(id);
            if (tag == null)
                return Result<bool>.NotFound(Messages.TagRetrieval.TagNotFound);

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
