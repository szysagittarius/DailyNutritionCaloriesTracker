using Microsoft.AspNetCore.Mvc;
using NutritionTracker.Api.Contracts.Common;
using NutritionTracker.Api.Contracts.FoodNutrition;
using NutritionTracker.Application.UseCases.Nutrition;

namespace NutritionTracker.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FoodNutritionController : ControllerBase
{
    private readonly GetAllFoodNutritionUseCase _getAllFoodNutritionUseCase;
    private readonly CreateFoodNutritionUseCase _createFoodNutritionUseCase;
    private readonly ILogger<FoodNutritionController> _logger;

    public FoodNutritionController(
        GetAllFoodNutritionUseCase getAllFoodNutritionUseCase,
        CreateFoodNutritionUseCase createFoodNutritionUseCase,
        ILogger<FoodNutritionController> logger)
    {
        _getAllFoodNutritionUseCase = getAllFoodNutritionUseCase;
        _createFoodNutritionUseCase = createFoodNutritionUseCase;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<FoodNutritionResponse>>>> GetAll()
    {
        try
        {
            var foodNutritions = await _getAllFoodNutritionUseCase.ExecuteAsync();
            var response = foodNutritions.Select(fn => new FoodNutritionResponse
            {
                Id = fn.Id,
                Name = fn.Name,
                Measurement = fn.Measurement,
                Carbs = fn.Carbs,
                Fat = fn.Fat,
                Protein = fn.Protein,
                Calories = fn.Calories
            });

            return Ok(ApiResponse<IEnumerable<FoodNutritionResponse>>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching all food nutritions");
            return StatusCode(500, ApiResponse<IEnumerable<FoodNutritionResponse>>.FailureResult("An error occurred while fetching food nutritions"));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<FoodNutritionResponse>>> Create([FromBody] CreateFoodNutritionRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<FoodNutritionResponse>.FailureResult("Invalid request data"));

            var foodNutrition = await _createFoodNutritionUseCase.ExecuteAsync(
                request.Name,
                request.Measurement,
                request.Carbs,
                request.Fat,
                request.Protein,
                request.Calories);

            var response = new FoodNutritionResponse
            {
                Id = foodNutrition.Id,
                Name = foodNutrition.Name,
                Measurement = foodNutrition.Measurement,
                Carbs = foodNutrition.Carbs,
                Fat = foodNutrition.Fat,
                Protein = foodNutrition.Protein,
                Calories = foodNutrition.Calories
            };

            _logger.LogInformation("Created food nutrition with ID {FoodNutritionId}", foodNutrition.Id);
            return Ok(ApiResponse<FoodNutritionResponse>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating food nutrition");
            return StatusCode(500, ApiResponse<FoodNutritionResponse>.FailureResult("An error occurred while creating the food nutrition"));
        }
    }
}
