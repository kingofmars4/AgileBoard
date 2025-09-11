using AgileBoard.API.DTOs;
using AgileBoard.Domain.Entities;
using AgileBoard.Services.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController(IProjectService projectService, IMapper mapper) : ControllerBase
{
    private readonly IProjectService _projectService = projectService;
    private readonly IMapper _mapper = mapper;

    [HttpPost]
    public async Task<IActionResult> CreateProject(CreateProjectDTO createDto)
    {
        var project = _mapper.Map<Project>(createDto);

        var createdProject = await _projectService.CreateProjectAsync(project);

        var projectDto = _mapper.Map<ProjectDTO>(createdProject);

        return Ok(projectDto);
    }
}