using AgileBoard.API;
using AgileBoard.API.DTOs;
using AgileBoard.Domain.Constants;
using AgileBoard.Services.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
public class UsersController(IUserService userService, IMapper mapper) : CustomController
{
    private readonly IUserService _userService = userService;
    private readonly IMapper _mapper = mapper;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDTO loginDto)
    {
        var result = await _userService.VerifyLoginAsync(loginDto.Username, loginDto.Password);

        return HandleResult(result, () => Ok(new { Message = Messages.Authentication.LoginSuccessful, loginDto.Username }));
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateUser(CreateUserDTO createUserDto)
    {
        var result = await _userService.RegisterUserAsync(createUserDto.Username, createUserDto.Email, createUserDto.Password);
        
        return HandleResult(result, user =>
        {
            var userDto = _mapper.Map<UserDTO>(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, userDto);
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _userService.GetAllUsersAsync();

        return HandleResult(result, users =>
        {
            var usersDTOs = _mapper.Map<IEnumerable<UserDTO>>(users);
            return Ok(usersDTOs);
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var result = await _userService.GetUserByIdAsync(id);
        
        return HandleResult(result, user =>
        {
            var userDto = _mapper.Map<UserDTO>(user);
            return Ok(userDto);
        });
    }

    [HttpGet("by-username/{username}")]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        var result = await _userService.GetUserByUsernameAsync(username);
        
        return HandleResult(result, user =>
        {
            var userDto = _mapper.Map<UserDTO>(user);
            return Ok(userDto);
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, UpdateUserDTO updateUserDto)
    {
        var result = await _userService.UpdateUserAsync(id, updateUserDto.Username, updateUserDto.Email);
        
        return HandleResult(result, user =>
        {
            var userDto = _mapper.Map<UserDTO>(user);
            return Ok(userDto);
        });
    }

    [HttpPut("{id:int}/change-password")]
    public async Task<IActionResult> ChangePassword(int id, ChangePasswordDTO changePasswordDto)
    {
        var result = await _userService.ChangePasswordAsync(id, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
        
        return HandleResult(result, () => 
            Ok(new { Message = Messages.PasswordChange.PasswordChangedSuccessfully }));
    }
}