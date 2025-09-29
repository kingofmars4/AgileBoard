using AgileBoard.API.DTOs;
using AgileBoard.API;
using AgileBoard.Services.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IAuthorizationService = AgileBoard.Services.Security.Interfaces.IAuthorizationService;
using AgileBoard.Domain.Constants;
using AgileBoard.Domain.Entities;

namespace AgileBoard.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkItemController(IWorkItemService workItemService, IAuthorizationService authService, IMapper mapper) 
        : CustomController
    {
        private readonly IWorkItemService _workItemService = workItemService;
        private readonly IAuthorizationService _authService = authService;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllWorkItems()
        {
            var result = await _workItemService.GetAllWorkItemsAsync();

            return HandleResult(result, workItems =>
            {
                var workItemDtos = _mapper.Map<IEnumerable<WorkItemSummaryDTO>>(workItems);
                return Ok(workItemDtos);
            });
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetWorkItemById(int id)
        {
            var result = await _workItemService.GetWorkItemByIdAsync(id);

            if (!result.IsSuccess)
                return HandleResult(result, _ => NotFound());

            var currentUserId = GetCurrentUserId();
            var authResult = await _authService.CanAccessProjectAsync(currentUserId, result.Data!.ProjectId);
            
            if (!authResult.IsSuccess)
                return HandleResult(authResult, _ => BadRequest());
                
            if (!authResult.Data)
                return StatusCode(403, Messages.WorkItemUpdate.NoPermissionToAccess);

            var workItemDto = _mapper.Map<WorkItemDTO>(result.Data);
            return Ok(workItemDto);
        }

        [HttpGet("project/{projectId:int}")]
        [Authorize]
        public async Task<IActionResult> GetWorkItemsByProject(int projectId)
        {
            var currentUserId = GetCurrentUserId();
            var authResult = await _authService.CanAccessProjectAsync(currentUserId, projectId);
            
            if (!authResult.IsSuccess)
                return HandleResult(authResult, _ => BadRequest());
                
            if (!authResult.Data)
                return StatusCode(403, Messages.WorkItemUpdate.NoPermissionToAccess);

            var result = await _workItemService.GetWorkItemsByProjectIdAsync(projectId);

            return HandleResult(result, workItems =>
            {
                var workItemDtos = _mapper.Map<IEnumerable<WorkItemSummaryDTO>>(workItems);
                return Ok(workItemDtos);
            });
        }

        [HttpGet("sprint/{sprintId:int}")]
        [Authorize]
        public async Task<IActionResult> GetWorkItemsBySprint(int sprintId)
        {
            var result = await _workItemService.GetWorkItemsBySprintIdAsync(sprintId);

            return HandleResult(result, workItems =>
            {
                var workItemDtos = _mapper.Map<IEnumerable<WorkItemSummaryDTO>>(workItems);
                return Ok(workItemDtos);
            });
        }

        [HttpGet("state/{state}")]
        [Authorize]
        public async Task<IActionResult> GetWorkItemsByState(WorkItemState state)
        {
            var domainState = (WorkItem.WorkItemState)state;
            var result = await _workItemService.GetWorkItemsByStateAsync(domainState);

            return HandleResult(result, workItems =>
            {
                var workItemDtos = _mapper.Map<IEnumerable<WorkItemSummaryDTO>>(workItems);
                return Ok(workItemDtos);
            });
        }

        [HttpGet("assigned/{userId:int}")]
        [Authorize]
        public async Task<IActionResult> GetWorkItemsByAssignedUser(int userId)
        {
            var result = await _workItemService.GetWorkItemsByAssignedUserIdAsync(userId);

            return HandleResult(result, workItems =>
            {
                var workItemDtos = _mapper.Map<IEnumerable<WorkItemSummaryDTO>>(workItems);
                return Ok(workItemDtos);
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateWorkItem(CreateWorkItemDTO createWorkItemDto)
        {
            var currentUserId = GetCurrentUserId();
            var authResult = await _authService.CanModifyProjectAsync(currentUserId, createWorkItemDto.ProjectId);
            
            if (!authResult.IsSuccess)
                return HandleResult(authResult, _ => BadRequest());
                
            if (!authResult.Data)
                return StatusCode(403, Messages.WorkItemUpdate.OnlyProjectMembersCanModify);

            var domainState = (WorkItem.WorkItemState)createWorkItemDto.State;
            var result = await _workItemService.CreateWorkItemAsync(
                createWorkItemDto.Name, 
                createWorkItemDto.Description, 
                createWorkItemDto.ProjectId,
                domainState,
                createWorkItemDto.SprintId);

            return HandleResult(result, workItem =>
            {
                var workItemDto = _mapper.Map<WorkItemDTO>(workItem);
                return CreatedAtAction(nameof(GetWorkItemById), new { id = workItem.Id }, workItemDto);
            });
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateWorkItem(int id, UpdateWorkItemDTO updateWorkItemDto)
        {
            var workItemResult = await _workItemService.GetWorkItemByIdAsync(id);
            if (!workItemResult.IsSuccess)
                return HandleResult(workItemResult, _ => NotFound());

            var currentUserId = GetCurrentUserId();
            var authResult = await _authService.CanModifyProjectAsync(currentUserId, workItemResult.Data!.ProjectId);
            
            if (!authResult.IsSuccess)
                return HandleResult(authResult, _ => BadRequest());
                
            if (!authResult.Data)
                return StatusCode(403, Messages.WorkItemUpdate.OnlyProjectMembersCanModify);

            var domainState = updateWorkItemDto.State.HasValue ? (WorkItem.WorkItemState)updateWorkItemDto.State.Value : (WorkItem.WorkItemState?)null;
            var result = await _workItemService.UpdateWorkItemAsync(
                id,
                updateWorkItemDto.Name,
                updateWorkItemDto.Description,
                domainState,
                updateWorkItemDto.Index,
                updateWorkItemDto.SprintId);

            return HandleResult(result, workItem =>
            {
                var workItemDto = _mapper.Map<WorkItemDTO>(workItem);
                return Ok(workItemDto);
            });
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteWorkItem(int id)
        {
            var workItemResult = await _workItemService.GetWorkItemByIdAsync(id);
            if (!workItemResult.IsSuccess)
                return HandleResult(workItemResult, _ => NotFound());

            var currentUserId = GetCurrentUserId();
            var authResult = await _authService.CanModifyProjectAsync(currentUserId, workItemResult.Data!.ProjectId);
            
            if (!authResult.IsSuccess)
                return HandleResult(authResult, _ => BadRequest());
                
            if (!authResult.Data)
                return StatusCode(403, Messages.WorkItemUpdate.OnlyProjectMembersCanModify);

            var result = await _workItemService.DeleteWorkItemAsync(id);

            return HandleResult(result, deleted =>
            {
                if (deleted)
                    return NoContent();
                else
                    return BadRequest("Failed to delete work item.");
            });
        }

        [HttpPost("{id:int}/assign")]
        [Authorize]
        public async Task<IActionResult> AssignUser(int id, AssignUserDTO assignUserDto)
        {
            var workItemResult = await _workItemService.GetWorkItemByIdAsync(id);
            if (!workItemResult.IsSuccess)
                return HandleResult(workItemResult, _ => NotFound());

            var currentUserId = GetCurrentUserId();
            var authResult = await _authService.CanModifyProjectAsync(currentUserId, workItemResult.Data!.ProjectId);
            
            if (!authResult.IsSuccess)
                return HandleResult(authResult, _ => BadRequest());
                
            if (!authResult.Data)
                return StatusCode(403, Messages.WorkItemUpdate.OnlyProjectMembersCanModify);

            var result = await _workItemService.AssignUserAsync(id, assignUserDto.UserId);

            return HandleResult(result, success =>
            {
                if (success)
                    return Ok(new { Message = Messages.WorkItemAssignment.UserAssignedSuccess });
                else
                    return BadRequest(Messages.WorkItemAssignment.AssignmentFailed);
            });
        }

        [HttpDelete("{id:int}/unassign/{userId:int}")]
        [Authorize]
        public async Task<IActionResult> UnassignUser(int id, int userId)
        {
            var workItemResult = await _workItemService.GetWorkItemByIdAsync(id);
            if (!workItemResult.IsSuccess)
                return HandleResult(workItemResult, _ => NotFound());

            var currentUserId = GetCurrentUserId();
            var authResult = await _authService.CanModifyProjectAsync(currentUserId, workItemResult.Data!.ProjectId);
            
            if (!authResult.IsSuccess)
                return HandleResult(authResult, _ => BadRequest());
                
            if (!authResult.Data)
                return StatusCode(403, Messages.WorkItemUpdate.OnlyProjectMembersCanModify);

            var result = await _workItemService.UnassignUserAsync(id, userId);

            return HandleResult(result, success =>
            {
                if (success)
                    return Ok(new { Message = Messages.WorkItemAssignment.UserUnassignedSuccess });
                else
                    return BadRequest(Messages.WorkItemAssignment.UnassignmentFailed);
            });
        }

        [HttpPost("{id:int}/tags")]
        [Authorize]
        public async Task<IActionResult> AddTag(int id, AddTagToWorkItemDTO addTagDto)
        {
            var workItemResult = await _workItemService.GetWorkItemByIdAsync(id);
            if (!workItemResult.IsSuccess)
                return HandleResult(workItemResult, _ => NotFound());

            var currentUserId = GetCurrentUserId();
            var authResult = await _authService.CanModifyProjectAsync(currentUserId, workItemResult.Data!.ProjectId);
            
            if (!authResult.IsSuccess)
                return HandleResult(authResult, _ => BadRequest());
                
            if (!authResult.Data)
                return StatusCode(403, Messages.WorkItemUpdate.OnlyProjectMembersCanModify);

            var result = await _workItemService.AddTagAsync(id, addTagDto.TagId);

            return HandleResult(result, success =>
            {
                if (success)
                    return Ok(new { Message = Messages.WorkItemTags.TagAddedSuccess });
                else
                    return BadRequest(Messages.WorkItemTags.AddTagFailed);
            });
        }

        [HttpDelete("{id:int}/tags/{tagId:int}")]
        [Authorize]
        public async Task<IActionResult> RemoveTag(int id, int tagId)
        {
            var workItemResult = await _workItemService.GetWorkItemByIdAsync(id);
            if (!workItemResult.IsSuccess)
                return HandleResult(workItemResult, _ => NotFound());

            var currentUserId = GetCurrentUserId();
            var authResult = await _authService.CanModifyProjectAsync(currentUserId, workItemResult.Data!.ProjectId);
            
            if (!authResult.IsSuccess)
                return HandleResult(authResult, _ => BadRequest());
                
            if (!authResult.Data)
                return StatusCode(403, Messages.WorkItemUpdate.OnlyProjectMembersCanModify);

            var result = await _workItemService.RemoveTagAsync(id, tagId);

            return HandleResult(result, success =>
            {
                if (success)
                    return Ok(new { Message = Messages.WorkItemTags.TagRemovedSuccess });
                else
                    return BadRequest(Messages.WorkItemTags.RemoveTagFailed);
            });
        }

        [HttpPut("{id:int}/sprint")]
        [Authorize]
        public async Task<IActionResult> MoveToSprint(int id, MoveToSprintDTO moveToSprintDto)
        {
            var workItemResult = await _workItemService.GetWorkItemByIdAsync(id);
            if (!workItemResult.IsSuccess)
                return HandleResult(workItemResult, _ => NotFound());

            var currentUserId = GetCurrentUserId();
            var authResult = await _authService.CanModifyProjectAsync(currentUserId, workItemResult.Data!.ProjectId);
            
            if (!authResult.IsSuccess)
                return HandleResult(authResult, _ => BadRequest());
                
            if (!authResult.Data)
                return StatusCode(403, Messages.WorkItemUpdate.OnlyProjectMembersCanModify);

            var result = await _workItemService.MoveToSprintAsync(id, moveToSprintDto.SprintId);

            return HandleResult(result, success =>
            {
                if (success)
                    return Ok(new { Message = Messages.WorkItems.WorkItemMovedSuccessfully });
                else
                    return BadRequest("Failed to move work item.");
            });
        }

        [HttpPut("{id:int}/index")]
        [Authorize]
        public async Task<IActionResult> UpdateIndex(int id, UpdateIndexDTO updateIndexDto)
        {
            var workItemResult = await _workItemService.GetWorkItemByIdAsync(id);
            if (!workItemResult.IsSuccess)
                return HandleResult(workItemResult, _ => NotFound());

            var currentUserId = GetCurrentUserId();
            var authResult = await _authService.CanModifyProjectAsync(currentUserId, workItemResult.Data!.ProjectId);
            
            if (!authResult.IsSuccess)
                return HandleResult(authResult, _ => BadRequest());
                
            if (!authResult.Data)
                return StatusCode(403, Messages.WorkItemUpdate.OnlyProjectMembersCanModify);

            var result = await _workItemService.UpdateIndexAsync(id, updateIndexDto.NewIndex);

            return HandleResult(result, success =>
            {
                if (success)
                    return Ok(new { Message = "Work item index updated successfully." });
                else
                    return BadRequest("Failed to update work item index.");
            });
        }
    }
}