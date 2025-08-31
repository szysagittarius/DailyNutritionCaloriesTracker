using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NT.Application.Contracts.Entities;
using NT.Application.Services.Abstractions;
using NutritionTracker.Api.Models;
using NutritionTracker.Api.Profilers;

namespace DailyNutritionCaloriesTracker.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{

    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UserController(ILogger<UserController> logger,
        IUserService userService,
        IMapper mapper)
    {
        _logger = logger;
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet("getusers")]
    public async Task<IEnumerable<UserDto>> GetAsync()
    {
        MapperConfiguration dtoMapperConfig = new MapperConfiguration(cfg =>
        {

            cfg.AddProfile(new UserDtoProfiler());
        });

        IMapper dtoMapper = dtoMapperConfig.CreateMapper();

        //load FoodLogDto from database by calling the service
        IEnumerable<UserEntity> entities = await _userService.GetAllAsync();

        IEnumerable<UserDto> userDtos = entities.Select(e => dtoMapper.Map<UserDto>(e));

        return userDtos;
    }

    [HttpPost("createuser")]
    public async Task<IActionResult> PostAsync([FromBody] UserDto userDto)
    {
        //insert the UserDto to the database by calling the service
        UserEntity entity = new UserEntity { Name = userDto.Name, Email = userDto.Email, Password = userDto.Password };
        await _userService.AddAsync(entity);
        return Ok(new { message = "User created successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDto loginDto)
    {
        try
        {
            // Validate input
            if (string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest(new { message = "Username and password are required" });
            }

            // Find user by username and verify password
            var users = await _userService.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Name == loginDto.Username && u.Password == loginDto.Password);
            
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }
            
            return Ok(new { message = "Login successful",  username = user.Name });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPut("updateprofile")]
    public async Task<IActionResult> UpdateProfileAsync([FromBody] UpdateUserProfileDto updateProfileDto)
    {
        try
        {
            // Validate input
            if (string.IsNullOrEmpty(updateProfileDto.Name))
            {
                return BadRequest(new { message = "Name is required" });
            }

            var users = await _userService.GetAllAsync();
            var existingUser = users.FirstOrDefault(u => u.Name == updateProfileDto.Name);
            
            if (existingUser == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Modify the existing entity's properties directly
            existingUser.Name = updateProfileDto.Name;
            existingUser.Email = updateProfileDto.Email;
            existingUser.SuggestedCalories = updateProfileDto.SuggestedCalories;
            
            // Only update password if provided
            if (!string.IsNullOrEmpty(updateProfileDto.Password))
            {
                existingUser.Password = updateProfileDto.Password;
            }

            // Update the modified entity
            UserEntity result = await _userService.UpdateAsync(existingUser);
            
            return Ok(new { 
                message = "Profile updated successfully",
                user = new {
                    name = result.Name,
                    email = result.Email,
                    suggestedCalories = result.SuggestedCalories
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("profile/{username}")]
    public async Task<IActionResult> GetProfileAsync(string username)
    {
        try
        {
            var users = await _userService.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Name == username);
            
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            
            return Ok(new {
                name = user.Name,
                email = user.Email,
                suggestedCalories = user.SuggestedCalories
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}
