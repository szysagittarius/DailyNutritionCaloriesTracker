using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NutritionTracker.Api.Contracts.Common;
using NutritionTracker.Api.Contracts.Requests;
using NutritionTracker.Api.Contracts.Responses;
using NutritionTracker.Application.UseCases.FoodLogs;
using NutritionTracker.Application.UseCases.FoodLogs.Queries;
using System.Net;
using System.Text.Json;

namespace NutritionTracker.AzureFunctions.Functions;

public class FoodLogFunctions
{
    private readonly ILogger<FoodLogFunctions> _logger;
    private readonly GetAllFoodLogsUseCase _getAllFoodLogsUseCase;
    private readonly GetFoodLogsByUserUseCase _getFoodLogsByUserUseCase;
    private readonly CreateFoodLogUseCase _createFoodLogUseCase;
    private readonly UpdateFoodLogUseCase _updateFoodLogUseCase;
    private readonly DeleteFoodLogUseCase _deleteFoodLogUseCase;

    public FoodLogFunctions(
        ILogger<FoodLogFunctions> logger,
        GetAllFoodLogsUseCase getAllFoodLogsUseCase,
        GetFoodLogsByUserUseCase getFoodLogsByUserUseCase,
        CreateFoodLogUseCase createFoodLogUseCase,
        UpdateFoodLogUseCase updateFoodLogUseCase,
        DeleteFoodLogUseCase deleteFoodLogUseCase)
    {
        _logger = logger;
        _getAllFoodLogsUseCase = getAllFoodLogsUseCase;
        _getFoodLogsByUserUseCase = getFoodLogsByUserUseCase;
        _createFoodLogUseCase = createFoodLogUseCase;
        _updateFoodLogUseCase = updateFoodLogUseCase;
        _deleteFoodLogUseCase = deleteFoodLogUseCase;
    }

    [Function("GetFoodLogs")]
    public async Task<HttpResponseData> GetFoodLogs(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "foodlog")] HttpRequestData req)
    {
        try
        {
            var foodLogs = await _getAllFoodLogsUseCase.ExecuteAsync();
            var response = foodLogs.Select(FoodLogResponse.FromDto);

            return await CreateJsonResponse(req, HttpStatusCode.OK,
                ApiResponse<IEnumerable<FoodLogResponse>>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting food logs");
            return await CreateJsonResponse(req, HttpStatusCode.InternalServerError,
                ApiResponse<IEnumerable<FoodLogResponse>>.FailureResult("An error occurred"));
        }
    }

    [Function("GetFoodLogsByUserId")]
    public async Task<HttpResponseData> GetFoodLogsByUserId(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "foodlog/user/{userId}")] HttpRequestData req,
        string userId)
    {
        try
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                return await CreateJsonResponse(req, HttpStatusCode.BadRequest,
                    ApiResponse<IEnumerable<FoodLogResponse>>.FailureResult("Invalid user ID"));
            }

            var foodLogs = await _getFoodLogsByUserUseCase.ExecuteAsync(new GetFoodLogsByUserQuery(userGuid));
            var response = foodLogs.Select(FoodLogResponse.FromDto);

            return await CreateJsonResponse(req, HttpStatusCode.OK,
                ApiResponse<IEnumerable<FoodLogResponse>>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting food logs for user {UserId}", userId);
            return await CreateJsonResponse(req, HttpStatusCode.InternalServerError,
                ApiResponse<IEnumerable<FoodLogResponse>>.FailureResult("An error occurred"));
        }
    }

    [Function("CreateFoodLog")]
    public async Task<HttpResponseData> CreateFoodLog(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "foodlog")] HttpRequestData req)
    {
        try
        {
            var body = await req.ReadAsStringAsync();
            var request = JsonSerializer.Deserialize<CreateFoodLogRequest>(body!, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (request == null)
            {
                return await CreateJsonResponse(req, HttpStatusCode.BadRequest,
                    ApiResponse<FoodLogResponse>.FailureResult("Invalid request data"));
            }

            var command = request.ToCommand();
            var foodLogDto = await _createFoodLogUseCase.ExecuteAsync(command);
            var response = FoodLogResponse.FromDto(foodLogDto);

            return await CreateJsonResponse(req, HttpStatusCode.Created,
                ApiResponse<FoodLogResponse>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating food log");
            return await CreateJsonResponse(req, HttpStatusCode.InternalServerError,
                ApiResponse<FoodLogResponse>.FailureResult("An error occurred"));
        }
    }

    [Function("UpdateFoodLog")]
    public async Task<HttpResponseData> UpdateFoodLog(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "foodlog/{id}")] HttpRequestData req,
        string id)
    {
        try
        {
            if (!Guid.TryParse(id, out var foodLogId))
            {
                return await CreateJsonResponse(req, HttpStatusCode.BadRequest,
                    ApiResponse<FoodLogResponse>.FailureResult("Invalid food log ID"));
            }

            var body = await req.ReadAsStringAsync();
            var request = JsonSerializer.Deserialize<CreateFoodLogRequest>(body!, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (request == null)
            {
                return await CreateJsonResponse(req, HttpStatusCode.BadRequest,
                    ApiResponse<FoodLogResponse>.FailureResult("Invalid request data"));
            }

            var foodItems = request.FoodItems.Select(fi => new Application.UseCases.FoodLogs.Commands.FoodItemDto(
                fi.FoodNutritionId,
                fi.Unit
            )).ToList();

            var foodLogDto = await _updateFoodLogUseCase.ExecuteAsync(foodLogId, request.DateTime, foodItems);
            var response = FoodLogResponse.FromDto(foodLogDto);

            return await CreateJsonResponse(req, HttpStatusCode.OK,
                ApiResponse<FoodLogResponse>.SuccessResult(response));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Food log {FoodLogId} not found", id);
            return await CreateJsonResponse(req, HttpStatusCode.NotFound,
                ApiResponse<FoodLogResponse>.FailureResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating food log {FoodLogId}", id);
            return await CreateJsonResponse(req, HttpStatusCode.InternalServerError,
                ApiResponse<FoodLogResponse>.FailureResult("An error occurred"));
        }
    }

    [Function("DeleteFoodLog")]
    public async Task<HttpResponseData> DeleteFoodLog(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "foodlog/{id}")] HttpRequestData req,
        string id)
    {
        try
        {
            if (!Guid.TryParse(id, out var foodLogId))
            {
                return await CreateJsonResponse(req, HttpStatusCode.BadRequest,
                    ApiResponse<bool>.FailureResult("Invalid food log ID"));
            }

            await _deleteFoodLogUseCase.ExecuteAsync(foodLogId);

            return await CreateJsonResponse(req, HttpStatusCode.OK,
                ApiResponse<bool>.SuccessResult(true));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Food log {FoodLogId} not found", id);
            return await CreateJsonResponse(req, HttpStatusCode.NotFound,
                ApiResponse<bool>.FailureResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting food log {FoodLogId}", id);
            return await CreateJsonResponse(req, HttpStatusCode.InternalServerError,
                ApiResponse<bool>.FailureResult("An error occurred"));
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
