using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NutritionTracker.Api.Contracts.Common;
using NutritionTracker.Api.Contracts.Users;
using NutritionTracker.Application.UseCases.Users;
using System.Net;
using System.Text.Json;

namespace NutritionTracker.AzureFunctions.Functions;

public class UserFunctions
{
    private readonly ILogger<UserFunctions> _logger;
    private readonly GetAllUsersUseCase _getAllUsersUseCase;
    private readonly GetUserByIdUseCase _getUserByIdUseCase;
    private readonly GetUserByUsernameUseCase _getUserByUsernameUseCase;
    private readonly CreateUserUseCase _createUserUseCase;
    private readonly UpdateUserUseCase _updateUserUseCase;

    public UserFunctions(
        ILogger<UserFunctions> logger,
        GetAllUsersUseCase getAllUsersUseCase,
        GetUserByIdUseCase getUserByIdUseCase,
        GetUserByUsernameUseCase getUserByUsernameUseCase,
        CreateUserUseCase createUserUseCase,
        UpdateUserUseCase updateUserUseCase)
    {
        _logger = logger;
        _getAllUsersUseCase = getAllUsersUseCase;
        _getUserByIdUseCase = getUserByIdUseCase;
        _getUserByUsernameUseCase = getUserByUsernameUseCase;
        _createUserUseCase = createUserUseCase;
        _updateUserUseCase = updateUserUseCase;
    }

    [Function("GetUsers")]
    public async Task<HttpResponseData> GetUsers(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users")] HttpRequestData req)
    {
        try
        {
            var users = await _getAllUsersUseCase.ExecuteAsync();
            var response = users.Select(u => new UserResponse
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                SuggestedCalories = u.SuggestedCalories,
                SuggestedCarbs = u.SuggestedCarbs,
                SuggestedFat = u.SuggestedFat,
                SuggestedProtein = u.SuggestedProtein
            });

            return await CreateJsonResponse(req, HttpStatusCode.OK, 
                ApiResponse<IEnumerable<UserResponse>>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return await CreateJsonResponse(req, HttpStatusCode.InternalServerError,
                ApiResponse<IEnumerable<UserResponse>>.FailureResult("An error occurred"));
        }
    }

    [Function("GetUserById")]
    public async Task<HttpResponseData> GetUserById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/{id}")] HttpRequestData req,
        string id)
    {
        try
        {
            if (!Guid.TryParse(id, out var userId))
            {
                return await CreateJsonResponse(req, HttpStatusCode.BadRequest,
                    ApiResponse<UserResponse>.FailureResult("Invalid user ID"));
            }

            var user = await _getUserByIdUseCase.ExecuteAsync(userId);
            if (user == null)
            {
                return await CreateJsonResponse(req, HttpStatusCode.NotFound,
                    ApiResponse<UserResponse>.FailureResult($"User with ID {id} not found"));
            }

            var response = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                SuggestedCalories = user.SuggestedCalories,
                SuggestedCarbs = user.SuggestedCarbs,
                SuggestedFat = user.SuggestedFat,
                SuggestedProtein = user.SuggestedProtein
            };

            return await CreateJsonResponse(req, HttpStatusCode.OK,
                ApiResponse<UserResponse>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {UserId}", id);
            return await CreateJsonResponse(req, HttpStatusCode.InternalServerError,
                ApiResponse<UserResponse>.FailureResult("An error occurred"));
        }
    }

    [Function("GetUserByUsername")]
    public async Task<HttpResponseData> GetUserByUsername(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/username/{username}")] HttpRequestData req,
        string username)
    {
        try
        {
            var user = await _getUserByUsernameUseCase.ExecuteAsync(username);
            if (user == null)
            {
                return await CreateJsonResponse(req, HttpStatusCode.NotFound,
                    ApiResponse<UserResponse>.FailureResult($"User '{username}' not found"));
            }

            var response = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                SuggestedCalories = user.SuggestedCalories,
                SuggestedCarbs = user.SuggestedCarbs,
                SuggestedFat = user.SuggestedFat,
                SuggestedProtein = user.SuggestedProtein
            };

            return await CreateJsonResponse(req, HttpStatusCode.OK,
                ApiResponse<UserResponse>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {Username}", username);
            return await CreateJsonResponse(req, HttpStatusCode.InternalServerError,
                ApiResponse<UserResponse>.FailureResult("An error occurred"));
        }
    }

    [Function("CreateUser")]
    public async Task<HttpResponseData> CreateUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users")] HttpRequestData req)
    {
        try
        {
            var body = await req.ReadAsStringAsync();
            var request = JsonSerializer.Deserialize<CreateUserRequest>(body!, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (request == null)
            {
                return await CreateJsonResponse(req, HttpStatusCode.BadRequest,
                    ApiResponse<UserResponse>.FailureResult("Invalid request data"));
            }

            var user = await _createUserUseCase.ExecuteAsync(
                request.Name,
                request.Email,
                request.Password,
                request.SuggestedCalories,
                request.SuggestedCarbs,
                request.SuggestedFat,
                request.SuggestedProtein);

            var response = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                SuggestedCalories = user.SuggestedCalories,
                SuggestedCarbs = user.SuggestedCarbs,
                SuggestedFat = user.SuggestedFat,
                SuggestedProtein = user.SuggestedProtein
            };

            return await CreateJsonResponse(req, HttpStatusCode.Created,
                ApiResponse<UserResponse>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return await CreateJsonResponse(req, HttpStatusCode.InternalServerError,
                ApiResponse<UserResponse>.FailureResult("An error occurred"));
        }
    }

    [Function("UpdateUser")]
    public async Task<HttpResponseData> UpdateUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "users/{id}")] HttpRequestData req,
        string id)
    {
        try
        {
            if (!Guid.TryParse(id, out var userId))
            {
                return await CreateJsonResponse(req, HttpStatusCode.BadRequest,
                    ApiResponse<UserResponse>.FailureResult("Invalid user ID"));
            }

            var body = await req.ReadAsStringAsync();
            var request = JsonSerializer.Deserialize<UpdateUserRequest>(body!, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (request == null)
            {
                return await CreateJsonResponse(req, HttpStatusCode.BadRequest,
                    ApiResponse<UserResponse>.FailureResult("Invalid request data"));
            }

            var user = await _updateUserUseCase.ExecuteAsync(
                userId,
                request.Name,
                request.Email,
                request.Password,
                request.SuggestedCalories,
                request.SuggestedCarbs,
                request.SuggestedFat,
                request.SuggestedProtein);

            var response = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                SuggestedCalories = user.SuggestedCalories,
                SuggestedCarbs = user.SuggestedCarbs,
                SuggestedFat = user.SuggestedFat,
                SuggestedProtein = user.SuggestedProtein
            };

            return await CreateJsonResponse(req, HttpStatusCode.OK,
                ApiResponse<UserResponse>.SuccessResult(response));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User {UserId} not found", id);
            return await CreateJsonResponse(req, HttpStatusCode.NotFound,
                ApiResponse<UserResponse>.FailureResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return await CreateJsonResponse(req, HttpStatusCode.InternalServerError,
                ApiResponse<UserResponse>.FailureResult("An error occurred"));
        }
    }

    private static async Task<HttpResponseData> CreateJsonResponse<T>(
        HttpRequestData req, HttpStatusCode statusCode, T data)
    {
        var response = req.CreateResponse(statusCode);
        response.Headers.Add("Content-Type", "application/json");
        await response.WriteStringAsync(JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
        return response;
    }
}
