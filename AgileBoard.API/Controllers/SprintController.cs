using AgileBoard.API.DTOs;
using AgileBoard.API;
using AgileBoard.Services.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IAuthorizationService = AgileBoard.Services.Security.Interfaces.IAuthorizationService;
using AgileBoard.Domain.Constants;

namespace AgileBoard.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SprintController(ISprintService sprintService, IAuthorizationService authService, IMapper mapper) 
        : CustomController
    {
        private readonly ISprintService _sprintService = sprintService;
        private readonly IAuthorizationService _authService = authService;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllSprints()
        {
            var result = await _sprintService.GetAllSprintsAsync();

            return HandleResult(result, sprints =>
            {
                var sprintDtos = _mapper.Map<IEnumerable<SprintSummaryDTO>>(sprints);
                return Ok(sprintDtos);
            });
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetSprintById(int id)
        {
            var result = await _sprintService.GetSprintByIdAsync(id);

            if (!result.IsSuccess)
                return HandleResult(result, _ => NotFound());

            var currentUserId = GetCurrentUserId();
            var authResult = await _authService.CanAccessProjectAsync(currentUserId, result.Data!.ProjectId);
            
            if (!authResult.IsSuccess)
                return HandleResult(authResult, _ => BadRequest());
                
            if (!authResult.Data)
                return StatusCode(403, Messages.SprintUpdate.NoPermissionToAccess);

            var sprintDto = _mapper.Map<SprintDTO>(result.Data);
            return Ok(sprintDto);
        }

        [HttpGet("project/{projectId:int}")]
        [Authorize]
        public async Task<IActionResult> GetSprintsByProject(int projectId)
        {
            var currentUserId = GetCurrentUserId();
            var authResult = await _authService.CanAccessProjectAsync(currentUserId, projectId);
            
            if (!authResult.IsSuccess)
                return HandleResult(authResult, _ => BadRequest());
                
            if (!authResult.Data)
                return StatusCode(403, Messages.SprintUpdate.NoPermissionToAccess);

            var result = await _sprintService.GetSprintsByProjectIdAsync(projectId);

            return HandleResult(result, sprints =>
            {
                var sprintDtos = _mapper.Map<IEnumerable<SprintSummaryDTO>>(sprints);
                return Ok(sprintDtos);
            });
        }

        [HttpGet("active")]
        [Authorize]
        public async Task<IActionResult> GetActiveSprints()
        {
            var result = await _sprintService.GetActiveSprintsAsync();

            return HandleResult(result, sprints =>
            {
                var sprintDtos = _mapper.Map<IEnumerable<SprintSummaryDTO>>(sprints);
                return Ok(sprintDtos);
            });
        }

        [HttpGet("date-range")]
        [Authorize]
        public async Task<IActionResult> GetSprintsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _sprintService.GetSprintsByDateRangeAsync(startDate, endDate);

            return HandleResult(result, sprints =>
            {
                var sprintDtos = _mapper.Map<IEnumerable<SprintSummaryDTO>>(sprints);
                return Ok(sprintDtos);
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateSprint(CreateSprintDTO createSprintDto)
        {
            var currentUserId = GetCurrentUserId();
            var authResult = await _authService.CanModifyProjectAsync(currentUserId, createSprintDto.ProjectId);
            
            if (!authResult.IsSuccess)
                return HandleResult(authResult, _ => BadRequest());
                
            if (!authResult.Data)
                return StatusCode(403, Messages.SprintUpdate.OnlyProjectMembersCanModify);

            var result = await _sprintService.CreateSprintAsync(
                createSprintDto.Name,
                createSprintDto.Description,
                createSprintDto.ProjectId,
                createSprintDto.StartDate,
                createSprintDto.EndDate);

            return HandleResult(result, sprint =>
            {
                var sprintDto = _mapper.Map<SprintDTO>(sprint);
                return CreatedAtAction(nameof(GetSprintById), new { id = sprint.Id }, sprintDto);
            });
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateSprint(int id, UpdateSprintDTO updateSprintDto)
        {
            var sprintResult = await _sprintService.GetSprintByIdAsync(id);
            if (!sprintResult.IsSuccess)
                return HandleResult(sprintResult, _ => NotFound());

            var currentUserId = GetCurrentUserId();
            var authResult = await _authService.CanAccessProjectAsync(currentUserId, sprintResult.Data!.ProjectId);
            
            if (!authResult.IsSuccess)
                return HandleResult(authResult, _ => BadRequest());
                
            if (!authResult.Data)
                return StatusCode(403, Messages.SprintUpdate.OnlyProjectMembersCanModify);

            var result = await _sprintService.UpdateSprintAsync(
                id,
                updateSprintDto.Name,
                updateSprintDto.Description,
                updateSprintDto.StartDate,
                updateSprintDto.EndDate);

            return HandleResult(result, sprint =>
            {
                var sprintDto = _mapper.Map<SprintDTO>(sprint);
                return Ok(sprintDto);
            });
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteSprint(int id)
        {
            var sprintResult = await _sprintService.GetSprintByIdAsync(id);
            if (!sprintResult.IsSuccess)
                return HandleResult(sprintResult, _ => NotFound());

            var currentUserId = GetCurrentUserId();
            var authResult = await _authService.CanModifyProjectAsync(currentUserId, sprintResult.Data!.ProjectId);
            
            if (!authResult.IsSuccess)
                return HandleResult(authResult, _ => BadRequest());
                
            if (!authResult.Data)
                return StatusCode(403, Messages.SprintUpdate.OnlyProjectMembersCanModify);

            var result = await _sprintService.DeleteSprintAsync(id);

            return HandleResult(result, deleted =>
            {
                if (deleted)
                    return NoContent();
                else
                    return BadRequest("Failed to delete sprint.");
            });
        }
    }
}