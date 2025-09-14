using AgileBoard.API;
using AgileBoard.API.DTOs;
using AgileBoard.Domain.Constants;
using AgileBoard.Services.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
public class UsersController(IUserService userService, IMapper mapper, AgileBoard.Services.Security.Interfaces.IAuthorizationService authService) 
    : CustomController
{
    private readonly IUserService _userService = userService;
    private readonly IMapper _mapper = mapper;
    private readonly AgileBoard.Services.Security.Interfaces.IAuthorizationService _authService = authService;

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginUserDTO loginDto)
    {
        var result = await _userService.VerifyLoginAsync(loginDto.Username, loginDto.Password);

        return HandleResult(result, data =>
        {
            var (user, token) = data;
            var userDto = _mapper.Map<UserDTO>(user);

            return Ok(new LoginResponseDTO(token, userDto, Messages.Authentication.LoginSuccessful));
        });
    }

    [HttpPost("register")]
    [AllowAnonymous]
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
    [Authorize]
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
    [Authorize]
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
    [Authorize]
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
    [Authorize]
    public async Task<IActionResult> UpdateUser(int id, UpdateUserDTO updateUserDto)
    {
        var authError = await CheckAuthorizationAsync(
            userId => _authService.CanModifyUserAsync(userId, id),
            "You can only modify your own profile");
        if (authError != null) return authError;
        
        var result = await _userService.UpdateUserAsync(id, updateUserDto.Username, updateUserDto.Email);
        
        return HandleResult(result, user =>
        {
            var userDto = _mapper.Map<UserDTO>(user);
            return Ok(userDto);
        });
    }

    [HttpPut("{id:int}/change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(int id, ChangePasswordDTO changePasswordDto)
    {
        var authError = await CheckAuthorizationAsync(
            userId => _authService.CanModifyUserAsync(userId, id),
            Messages.PasswordChange.NoPermissionToChangePassword);
        if (authError != null) return authError;
        
        var result = await _userService.ChangePasswordAsync(id, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
        
        return HandleResult(result, () => 
            Ok(new { Message = Messages.PasswordChange.PasswordChangedSuccessfully }));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var currentUserId = GetCurrentUserId();
        var result = await _userService.GetUserByIdAsync(currentUserId);

        return HandleResult(result, user =>
        {
            var userDto = _mapper.Map<UserDTO>(user);
            return Ok(userDto);
        });
    }
}