using Microsoft.AspNetCore.Mvc;
using NutritionTracker.Api.Contracts.Common;
using NutritionTracker.Api.Contracts.Users;
using NutritionTracker.Application.UseCases.UserRoles;
using NutritionTracker.Application.UseCases.Users;
using NutritionTracker.Domain.Entities;

namespace NutritionTracker.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly GetAllUsersUseCase _getAllUsersUseCase;
    private readonly GetUserByIdUseCase _getUserByIdUseCase;
    private readonly GetUserByUsernameUseCase _getUserByUsernameUseCase;
    private readonly CreateUserUseCase _createUserUseCase;
    private readonly UpdateUserUseCase _updateUserUseCase;
    private readonly DeleteUserUseCase _deleteUserUseCase;
    private readonly GetAllUserRolesUseCase _getAllUserRolesUseCase;
    private readonly ILogger<UserController> _logger;

    public UserController(
        GetAllUsersUseCase getAllUsersUseCase,
        GetUserByIdUseCase getUserByIdUseCase,
        GetUserByUsernameUseCase getUserByUsernameUseCase,
        CreateUserUseCase createUserUseCase,
        UpdateUserUseCase updateUserUseCase,
        DeleteUserUseCase deleteUserUseCase,
        GetAllUserRolesUseCase getAllUserRolesUseCase,
        ILogger<UserController> logger)
    {
        _getAllUsersUseCase = getAllUsersUseCase;
        _getUserByIdUseCase = getUserByIdUseCase;
        _getUserByUsernameUseCase = getUserByUsernameUseCase;
        _createUserUseCase = createUserUseCase;
        _updateUserUseCase = updateUserUseCase;
        _deleteUserUseCase = deleteUserUseCase;
        _getAllUserRolesUseCase = getAllUserRolesUseCase;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserResponse>>>> GetAll()
    {
        try
        {
            var users = await _getAllUsersUseCase.ExecuteAsync();
            var response = users.Select(u => new UserResponse
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                RoleId = u.RoleId,
                RoleName = u.RoleName,
                IsAdmin = u.IsAdmin,
                SuggestedCalories = u.SuggestedCalories,
                SuggestedCarbs = u.SuggestedCarbs,
                SuggestedFat = u.SuggestedFat,
                SuggestedProtein = u.SuggestedProtein
            });

            return Ok(ApiResponse<IEnumerable<UserResponse>>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching all users");
            return StatusCode(500, ApiResponse<IEnumerable<UserResponse>>.FailureResult("An error occurred while fetching users"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetById(Guid id)
    {
        try
        {
            var user = await _getUserByIdUseCase.ExecuteAsync(id);
            
            if (user == null)
                return NotFound(ApiResponse<UserResponse>.FailureResult($"User with ID {id} not found"));

            var response = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.RoleName,
                IsAdmin = user.IsAdmin,
                SuggestedCalories = user.SuggestedCalories,
                SuggestedCarbs = user.SuggestedCarbs,
                SuggestedFat = user.SuggestedFat,
                SuggestedProtein = user.SuggestedProtein
            };

            return Ok(ApiResponse<UserResponse>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching user with ID {UserId}", id);
            return StatusCode(500, ApiResponse<UserResponse>.FailureResult("An error occurred while fetching the user"));
        }
    }

    [HttpGet("username/{username}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetByUsername(string username)
    {
        try
        {
            var user = await _getUserByUsernameUseCase.ExecuteAsync(username);
            
            if (user == null)
                return NotFound(ApiResponse<UserResponse>.FailureResult($"User with username '{username}' not found"));

            var response = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.RoleName,
                IsAdmin = user.IsAdmin,
                SuggestedCalories = user.SuggestedCalories,
                SuggestedCarbs = user.SuggestedCarbs,
                SuggestedFat = user.SuggestedFat,
                SuggestedProtein = user.SuggestedProtein
            };

            return Ok(ApiResponse<UserResponse>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching user with username {Username}", username);
            return StatusCode(500, ApiResponse<UserResponse>.FailureResult("An error occurred while fetching the user"));
        }
    }

    [HttpGet("roles")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserRoleResponse>>>> GetRoles()
    {
        try
        {
            var roles = await _getAllUserRolesUseCase.ExecuteAsync();
            var response = roles.Select(r => new UserRoleResponse
            {
                Id = r.Id,
                Name = r.Name
            });

            return Ok(ApiResponse<IEnumerable<UserRoleResponse>>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching user roles");
            return StatusCode(500, ApiResponse<IEnumerable<UserRoleResponse>>.FailureResult("An error occurred while fetching roles"));
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request?.Username) || string.IsNullOrWhiteSpace(request?.Password))
                return BadRequest(new LoginResponse { Message = "Username and password are required" });

            var user = await _getUserByUsernameUseCase.ExecuteAsync(request.Username);
            if (user == null || user.Password != request.Password)
                return Unauthorized(new LoginResponse { Message = "Invalid username or password" });

            return Ok(new LoginResponse
            {
                Id = user.Id,
                Username = user.Name,
                RoleId = user.RoleId,
                RoleName = user.RoleName,
                IsAdmin = user.IsAdmin,
                Message = "Login successful"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while logging in user {Username}", request?.Username);
            return StatusCode(500, new LoginResponse { Message = "An error occurred while logging in" });
        }
    }

    [HttpPost]
    [HttpPost("createuser")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> Create([FromBody] CreateUserRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<UserResponse>.FailureResult("Invalid request data"));

            var user = await _createUserUseCase.ExecuteAsync(
                request.Name,
                request.Email,
                request.Password,
                request.SuggestedCalories,
                request.SuggestedCarbs,
                request.SuggestedFat,
                request.SuggestedProtein,
                request.RoleId ?? UserRole.DefaultUserRoleId);

            var response = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.RoleName,
                IsAdmin = user.IsAdmin,
                SuggestedCalories = user.SuggestedCalories,
                SuggestedCarbs = user.SuggestedCarbs,
                SuggestedFat = user.SuggestedFat,
                SuggestedProtein = user.SuggestedProtein
            };

            _logger.LogInformation("Created user with ID {UserId}", user.Id);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, ApiResponse<UserResponse>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating user");
            return StatusCode(500, ApiResponse<UserResponse>.FailureResult("An error occurred while creating the user"));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<UserResponse>.FailureResult("Invalid request data"));

            var user = await _updateUserUseCase.ExecuteAsync(
                id,
                request.Name,
                request.Email,
                request.Password,
                request.SuggestedCalories,
                request.SuggestedCarbs,
                request.SuggestedFat,
                request.SuggestedProtein,
                request.RoleId);

            var response = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.RoleName,
                IsAdmin = user.IsAdmin,
                SuggestedCalories = user.SuggestedCalories,
                SuggestedCarbs = user.SuggestedCarbs,
                SuggestedFat = user.SuggestedFat,
                SuggestedProtein = user.SuggestedProtein
            };

            _logger.LogInformation("Updated user with ID {UserId}", user.Id);
            return Ok(ApiResponse<UserResponse>.SuccessResult(response));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User with ID {UserId} not found", id);
            return NotFound(ApiResponse<UserResponse>.FailureResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating user with ID {UserId}", id);
            return StatusCode(500, ApiResponse<UserResponse>.FailureResult("An error occurred while updating the user"));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        try
        {
            await _deleteUserUseCase.ExecuteAsync(id);
            _logger.LogInformation("Deleted user with ID {UserId}", id);
            return Ok(ApiResponse<object>.SuccessResult("User deleted successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User with ID {UserId} not found for deletion", id);
            return NotFound(ApiResponse<object>.FailureResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting user with ID {UserId}", id);
            return StatusCode(500, ApiResponse<object>.FailureResult("An error occurred while deleting the user"));
        }
    }
}
