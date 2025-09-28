using AgileBoard.API.DTOs;
using AgileBoard.API;
using AgileBoard.Services.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgileBoard.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController(ITagService tagService, IMapper mapper) : CustomController
    {
        private readonly ITagService _tagService = tagService;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllTags()
        {
            var result = await _tagService.GetAllTagsAsync();

            return HandleResult(result, tags =>
            {
                var tagDtos = _mapper.Map<IEnumerable<TagDTO>>(tags);

                return Ok(tagDtos);
            });
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetTagById(int id)
        {
            var result = await _tagService.GetTagByIdAsync(id);

            return HandleResult(result, tag =>
            {
                var tagDto = _mapper.Map<TagDTO>(tag);
                return Ok(tagDto);
            });
        }

        [HttpGet("by-name/{name}")]
        [Authorize]
        public async Task<IActionResult> GetTagByName(string name)
        {
            var result = await _tagService.GetTagByNameAsync(name);

            return HandleResult(result, tag =>
            {
                var tagDto = _mapper.Map<TagDTO>(tag);
                return Ok(tagDto);
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTag(CreateTagDTO createTagDto)
        {
            var result = await _tagService.CreateTagAsync(createTagDto.Name);

            return HandleResult(result, tag =>
            {
                var tagDto = _mapper.Map<TagDTO>(tag);
                return CreatedAtAction(nameof(GetTagById), new { id = tag.Id }, tagDto);
            });
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateTag(int id, UpdateTagDTO updateTagDto)
        {
            var result = await _tagService.UpdateTagAsync(id, updateTagDto.NewName);

            return HandleResult(result, tag =>
            {
                var tagDto = _mapper.Map<TagDTO>(tag);
                return Ok(tagDto);
            });
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var result = await _tagService.DeleteTagAsync(id);

            return HandleResult(result, deleted =>
            {
                if (deleted)
                    return Ok();
                else
                    return BadRequest("Failed to delete tag.");
            });
        }
    }
}
