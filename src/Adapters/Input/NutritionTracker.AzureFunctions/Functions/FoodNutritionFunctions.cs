using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NutritionTracker.Api.Contracts.Common;
using NutritionTracker.Api.Contracts.FoodNutrition;
using NutritionTracker.Application.UseCases.Nutrition;
using System.Net;
using System.Text.Json;

namespace NutritionTracker.AzureFunctions.Functions;

public class FoodNutritionFunctions
{
    private readonly ILogger<FoodNutritionFunctions> _logger;
    private readonly GetAllFoodNutritionUseCase _getAllFoodNutritionUseCase;
    private readonly CreateFoodNutritionUseCase _createFoodNutritionUseCase;

    public FoodNutritionFunctions(
        ILogger<FoodNutritionFunctions> logger,
        GetAllFoodNutritionUseCase getAllFoodNutritionUseCase,
        CreateFoodNutritionUseCase createFoodNutritionUseCase)
    {
        _logger = logger;
        _getAllFoodNutritionUseCase = getAllFoodNutritionUseCase;
        _createFoodNutritionUseCase = createFoodNutritionUseCase;
    }

    [Function("GetFoodNutrition")]
    public async Task<HttpResponseData> GetFoodNutrition(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "foodnutrition")] HttpRequestData req)
    {
        try
        {
            var foods = await _getAllFoodNutritionUseCase.ExecuteAsync();
            var response = foods.Select(f => new FoodNutritionResponse
            {
                Id = f.Id,
                Name = f.Name,
                Measurement = f.Measurement,
                Calories = f.Calories,
                Carbs = f.Carbs,
                Protein = f.Protein,
                Fat = f.Fat
            });

            return await CreateJsonResponse(req, HttpStatusCode.OK,
                ApiResponse<IEnumerable<FoodNutritionResponse>>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting food nutrition");
            return await CreateJsonResponse(req, HttpStatusCode.InternalServerError,
                ApiResponse<IEnumerable<FoodNutritionResponse>>.FailureResult("An error occurred"));
        }
    }

    [Function("CreateFoodNutrition")]
    public async Task<HttpResponseData> CreateFoodNutrition(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "foodnutrition")] HttpRequestData req)
    {
        try
        {
            var body = await req.ReadAsStringAsync();
            var request = JsonSerializer.Deserialize<CreateFoodNutritionRequest>(body!, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (request == null)
            {
                return await CreateJsonResponse(req, HttpStatusCode.BadRequest,
                    ApiResponse<FoodNutritionResponse>.FailureResult("Invalid request data"));
            }

            var food = await _createFoodNutritionUseCase.ExecuteAsync(
                request.Name,
                request.Measurement,
                request.Carbs,
                request.Fat,
                request.Protein,
                request.Calories);

            var response = new FoodNutritionResponse
            {
                Id = food.Id,
                Name = food.Name,
                Measurement = food.Measurement,
                Calories = food.Calories,
                Carbs = food.Carbs,
                Protein = food.Protein,
                Fat = food.Fat
            };

            return await CreateJsonResponse(req, HttpStatusCode.Created,
                ApiResponse<FoodNutritionResponse>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating food nutrition");
            return await CreateJsonResponse(req, HttpStatusCode.InternalServerError,
                ApiResponse<FoodNutritionResponse>.FailureResult("An error occurred"));
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
