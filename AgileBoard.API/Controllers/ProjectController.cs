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
    public class ProjectController(IProjectService projectService, IAuthorizationService authService, IMapper mapper) 
        : CustomController
    {
        private readonly IProjectService _projectService = projectService;
        private readonly IAuthorizationService _authService = authService;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllProjects()
        {
            var currentUserId = GetCurrentUserId();
            
            var ownedProjectsResult = await _projectService.GetProjectsByOwnerIdAsync(currentUserId);
            var participantProjectsResult = await _projectService.GetProjectsByParticipantIdAsync(currentUserId);

            var allProjectDtos = new List<ProjectSummaryDTO>();
            
            if (ownedProjectsResult.IsSuccess)
            {
                var ownedDtos = _mapper.Map<IEnumerable<ProjectSummaryDTO>>(ownedProjectsResult.Data!);
                allProjectDtos.AddRange(ownedDtos);
            }
                
            if (participantProjectsResult.IsSuccess)
            {
                var participantDtos = _mapper.Map<IEnumerable<ProjectSummaryDTO>>(participantProjectsResult.Data!);
                allProjectDtos.AddRange(participantDtos);
            }

            var uniqueProjects = allProjectDtos.GroupBy(p => p.Id).Select(g => g.First());

            if (!uniqueProjects.Any())
                return NotFound(Messages.Projects.NoProjectsFoundForUser);

            return Ok(uniqueProjects);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetProjectById(int id)
        {
            var currentUserId = GetCurrentUserId();
            var authResult = await _authService.CanAccessProjectAsync(currentUserId, id);
            
            if (!authResult.IsSuccess)
                return HandleResult(authResult, _ => BadRequest());
                
            if (!authResult.Data)
                return Forbid(Messages.ProjectUpdate.NoPermissionToAccess);

            var result = await _projectService.GetProjectByIdAsync(id);

            return HandleResult(result, project =>
            {
                var projectDto = _mapper.Map<ProjectDTO>(project);
                return Ok(projectDto);
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProject(CreateProjectDTO createProjectDto)
        {
            var currentUserId = GetCurrentUserId();
            var result = await _projectService.CreateProjectAsync(
                createProjectDto.Name, 
                createProjectDto.Description, 
                currentUserId);

            return HandleResult(result, project =>
            {
                var projectDto = _mapper.Map<ProjectDTO>(project);
                return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, projectDto);
            });
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateProject(int id, UpdateProjectDTO updateProjectDto)
        {
            var currentUserId = GetCurrentUserId();
            var authResult = await _authService.CanModifyProjectAsync(currentUserId, id);
            
            if (!authResult.IsSuccess)
                return HandleResult(authResult, _ => BadRequest());
                
            if (!authResult.Data!)
                return Forbid(Messages.ProjectUpdate.OnlyOwnerCanUpdate);

            var result = await _projectService.UpdateProjectAsync(id, updateProjectDto.Name, updateProjectDto.Description);

            return HandleResult(result, project =>
            {
                var projectDto = _mapper.Map<ProjectDTO>(project);
                return Ok(projectDto);
            });
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var currentUserId = GetCurrentUserId();
            var authResult = await _authService.CanModifyProjectAsync(currentUserId, id);
            
            if (!authResult.IsSuccess)
                return HandleResult(authResult, _ => BadRequest());
                
            if (!authResult.Data!)
                return Forbid(Messages.ProjectUpdate.OnlyOwnerCanDelete);

            var result = await _projectService.DeleteProjectAsync(id);

            return HandleResult(result, deleted =>
            {
                if (deleted)
                    return NoContent();
                else
                    return BadRequest(Messages.Projects.ProjectDeleteFailed);
            });
        }

        [HttpPost("{id:int}/participants")]
        [Authorize]
        public async Task<IActionResult> AddParticipant(int id, AddParticipantDTO addParticipantDto)
        {
            var currentUserId = GetCurrentUserId();
            var authResult = await _authService.CanModifyProjectAsync(currentUserId, id);
            
            if (!authResult.IsSuccess)
                return HandleResult(authResult, _ => BadRequest());
                
            if (!authResult.Data!)
                return Forbid(Messages.ProjectUpdate.OnlyOwnerCanAddParticipants);

            var result = await _projectService.AddParticipantAsync(id, addParticipantDto.UserId);

            return HandleResult(result, success =>
            {
                if (success)
                    return Ok(new { Message = Messages.Participants.AddParticipantSuccess });
                else
                    return BadRequest(Messages.Participants.AddParticipantFailed);
            });
        }

        [HttpDelete("{id:int}/participants/{userId:int}")]
        [Authorize]
        public async Task<IActionResult> RemoveParticipant(int id, int userId)
        {
            var currentUserId = GetCurrentUserId();
            var authResult = await _authService.CanModifyProjectAsync(currentUserId, id);
            
            if (!authResult.IsSuccess)
                return HandleResult(authResult, _ => BadRequest());
                
            if (!authResult.Data!)
                return Forbid(Messages.ProjectUpdate.OnlyOwnerCanRemoveParticipants);

            var result = await _projectService.RemoveParticipantAsync(id, userId);

            return HandleResult(result, success =>
            {
                if (success)
                    return Ok(new { Message = Messages.Participants.RemoveParticipantSuccess });
                else
                    return BadRequest(Messages.Participants.RemoveParticipantFailed);
            });
        }

        [HttpGet("{id:int}/participants")]
        [Authorize]
        public async Task<IActionResult> GetProjectParticipants(int id)
        {
            var currentUserId = GetCurrentUserId();
            var authResult = await _authService.CanAccessProjectAsync(currentUserId, id);
            
            if (!authResult.IsSuccess)
                return HandleResult(authResult, _ => BadRequest());
                
            if (!authResult.Data!)
                return Forbid(Messages.ProjectUpdate.NoPermissionToAccess);

            var result = await _projectService.GetProjectByIdAsync(id);

            return HandleResult(result, project =>
            {
                var participants = project.Participants ?? Enumerable.Empty<object>(); 
                var participantDtos = _mapper.Map<IEnumerable<UserDTO>>(participants);
                return Ok(participantDtos);
            });
        }

        [HttpGet("owned")]
        [Authorize]
        public async Task<IActionResult> GetOwnedProjects()
        {
            var currentUserId = GetCurrentUserId();
            var result = await _projectService.GetProjectsByOwnerIdAsync(currentUserId);

            return HandleResult(result, projects =>
            {
                var projectDtos = _mapper.Map<IEnumerable<ProjectSummaryDTO>>(projects);
                return Ok(projectDtos);
            });
        }

        [HttpGet("participating")]
        [Authorize]
        public async Task<IActionResult> GetParticipatingProjects()
        {
            var currentUserId = GetCurrentUserId();
            var result = await _projectService.GetProjectsByParticipantIdAsync(currentUserId);

            return HandleResult(result, projects =>
            {
                var projectDtos = _mapper.Map<IEnumerable<ProjectSummaryDTO>>(projects);
                return Ok(projectDtos);
            });
        }
    }
}