using AgileBoard.API.DTOs;
using AgileBoard.API.Extensions;
using AgileBoard.Domain.Constants;
using AgileBoard.Services.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService, IMapper mapper) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IMapper _mapper = mapper;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDTO loginDto)
    {
        var result = await _userService.VerifyLoginAsync(loginDto.Username, loginDto.Password);
        
        var errorResult = result.ToActionResultIfFailed(this);
        if (errorResult != null) return errorResult;

        return Ok(new { Message = Messages.Authentication.LoginSuccessful, loginDto.Username });
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateUser(CreateUserDTO createUserDto)
    {
        var result = await _userService.RegisterUserAsync(createUserDto.Username, createUserDto.Email, createUserDto.Password);
        
        var errorResult = result.ToActionResultIfFailed(this);
        if (errorResult != null) return errorResult;

        var userDto = _mapper.Map<UserDTO>(result.Data);
        return CreatedAtAction(nameof(GetUserById), new { id = result.Data!.Id }, userDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _userService.GetAllUsersAsync();
        
        var errorResult = result.ToActionResultIfFailed(this);
        if (errorResult != null) return errorResult;

        var usersDTOs = _mapper.Map<IEnumerable<UserDTO>>(result.Data);
        return Ok(usersDTOs);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var result = await _userService.GetUserByIdAsync(id);
        
        var errorResult = result.ToActionResultIfFailed(this);
        if (errorResult != null) return errorResult;

        var userDto = _mapper.Map<UserDTO>(result.Data);
        return Ok(userDto);
    }

    [HttpGet("by-username/{username}")]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        var result = await _userService.GetUserByUsernameAsync(username);
        
        var errorResult = result.ToActionResultIfFailed(this);
        if (errorResult != null) return errorResult;

        var userDto = _mapper.Map<UserDTO>(result.Data);
        return Ok(userDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, UpdateUserDTO updateUserDto)
    {
        var result = await _userService.UpdateUserAsync(id, updateUserDto.Username, updateUserDto.Email);
        
        var errorResult = result.ToActionResultIfFailed(this);
        if (errorResult != null) return errorResult;

        var userDto = _mapper.Map<UserDTO>(result.Data);
        return Ok(userDto);
    }

    [HttpPut("{id:int}/change-password")]
    public async Task<IActionResult> ChangePassword(int id, ChangePasswordDTO changePasswordDto)
    {
        var result = await _userService.ChangePasswordAsync(id, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
        
        var errorResult = result.ToActionResultIfFailed(this);
        if (errorResult != null) return errorResult;

        return Ok(new { Message = Messages.PasswordChange.PasswordChangedSuccessfully });
    }
}