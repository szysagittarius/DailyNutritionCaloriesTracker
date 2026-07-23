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

    [Function("LoginUser")]
    public async Task<HttpResponseData> LoginUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/login")] HttpRequestData req)
    {
        return await HandleLogin(req);
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

    private async Task<HttpResponseData> HandleLogin(HttpRequestData req)
    {
        try
        {
            var body = await req.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(body))
            {
                return await CreateJsonResponse(req, HttpStatusCode.BadRequest,
                    new LoginResponse { Message = "Username and password are required" });
            }

            LoginRequest? request = ParseLoginRequest(body);

            if (string.IsNullOrWhiteSpace(request?.Username) || string.IsNullOrWhiteSpace(request?.Password))
            {
                return await CreateJsonResponse(req, HttpStatusCode.BadRequest,
                    new LoginResponse { Message = "Username and password are required" });
            }

            var user = await _getUserByUsernameUseCase.ExecuteAsync(request.Username);
            if (user == null || user.Password != request.Password)
            {
                return await CreateJsonResponse(req, HttpStatusCode.Unauthorized,
                    new LoginResponse { Message = "Invalid username or password" });
            }

            return await CreateJsonResponse(req, HttpStatusCode.OK,
                new LoginResponse
                {
                    Id = user.Id,
                    Username = user.Name,
                    Message = "Login successful"
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging in user");
            return await CreateJsonResponse(req, HttpStatusCode.InternalServerError,
                new LoginResponse { Message = "An error occurred while logging in" });
        }
    }

    [Function("CreateUser")]
    public async Task<HttpResponseData> CreateUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users")] HttpRequestData req)
    {
        return await HandleCreateUser(req);
    }

    private async Task<HttpResponseData> HandleCreateUser(HttpRequestData req)
    {
        try
        {
            var body = await req.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(body))
            {
                return await CreateJsonResponse(req, HttpStatusCode.BadRequest,
                    ApiResponse<UserResponse>.FailureResult("Invalid request data"));
            }

            CreateUserRequest? request = ParseCreateUserRequest(body);

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

    private static LoginRequest? ParseLoginRequest(string? body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<LoginRequest>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (JsonException)
        {
            try
            {
                using var document = JsonDocument.Parse(body);
                if (document.RootElement.ValueKind != JsonValueKind.Object)
                {
                    return null;
                }

                return new LoginRequest
                {
                    Username = GetStringProperty(document.RootElement, "username", "email") ?? string.Empty,
                    Password = GetStringProperty(document.RootElement, "password") ?? string.Empty
                };
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }

    private static CreateUserRequest? ParseCreateUserRequest(string? body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<CreateUserRequest>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (JsonException)
        {
            try
            {
                using var document = JsonDocument.Parse(body);
                if (document.RootElement.ValueKind != JsonValueKind.Object)
                {
                    return null;
                }

                return new CreateUserRequest
                {
                    Name = GetStringProperty(document.RootElement, "name", "username") ?? string.Empty,
                    Email = GetStringProperty(document.RootElement, "email") ?? string.Empty,
                    Password = GetStringProperty(document.RootElement, "password") ?? string.Empty,
                    SuggestedCalories = GetDoubleProperty(document.RootElement, "suggestedCalories"),
                    SuggestedCarbs = GetDoubleProperty(document.RootElement, "suggestedCarbs"),
                    SuggestedFat = GetDoubleProperty(document.RootElement, "suggestedFat"),
                    SuggestedProtein = GetDoubleProperty(document.RootElement, "suggestedProtein")
                };
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }

    private static string? GetStringProperty(JsonElement element, params string[] propertyNames)
    {
        foreach (var propertyName in propertyNames)
        {
            if (element.TryGetProperty(propertyName, out var propertyValue) && propertyValue.ValueKind == JsonValueKind.String)
            {
                return propertyValue.GetString();
            }
        }

        return null;
    }

    private static double GetDoubleProperty(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var propertyValue) && propertyValue.ValueKind is JsonValueKind.Number
            ? propertyValue.GetDouble()
            : 0;
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
