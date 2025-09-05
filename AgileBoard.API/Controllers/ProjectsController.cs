using AgileBoard.API.DTOs;
using AgileBoard.Domain.Entities;
using AgileBoard.Services.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IMapper _mapper;

    public ProjectsController(IProjectService projectService, IMapper mapper)
    {
        _projectService = projectService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject(CreateProjectDTO createDto)
    {
        var project = _mapper.Map<Project>(createDto);

        var createdProject = await _projectService.CreateProjectAsync(project);

        var projectDto = _mapper.Map<ProjectDTO>(createdProject);

        return Ok(projectDto);
    }
}