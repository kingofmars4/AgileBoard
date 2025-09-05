using AgileBoard.API.DTOs;
using AgileBoard.Domain.Entities;
using AgileBoard.Services.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UsersController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserDTO createUserDto)
    {
        var newUser = _mapper.Map<User>(createUserDto);

        var createdUser = await _userService.CreateUserAsync(newUser);

        var userDto = _mapper.Map<UserDTO>(createdUser);

        return Ok(userDto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        if (users.IsNullOrEmpty()) return NotFound();

        var usersDTOs = _mapper.Map<IEnumerable<UserDTO>>(users);
        return Ok(usersDTOs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDTO>> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null) return NotFound();
        
        var userDto = _mapper.Map<UserDTO>(user);
        return Ok(userDto);
    }

}