using AgileBoard.API.DTOs;
using AgileBoard.Services.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService, IMapper mapper) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IMapper _mapper = mapper;

    [HttpPost("register")]
    public async Task<IActionResult> CreateUser(CreateUserDTO createUserDto)
    {
        try
        {
            var newUser = await _userService.RegisterUserAsync(createUserDto.Username, createUserDto.Email, createUserDto.Password);

            var userDto = _mapper.Map<UserDTO>(newUser);

            return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, userDto);

        } 
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while registering the user.");
        }
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