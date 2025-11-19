using Microsoft.AspNetCore.Mvc;
using NutritionTracker.Api.Contracts.Common;
using NutritionTracker.Api.Contracts.Extensions;
using NutritionTracker.Api.Contracts.Requests;
using NutritionTracker.Api.Contracts.Responses;
using NutritionTracker.Application.UseCases.FoodLogs;
using NutritionTracker.Application.UseCases.FoodLogs.Commands;
using NutritionTracker.Application.UseCases.FoodLogs.Queries;

namespace NutritionTracker.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FoodLogController : ControllerBase
{
    private readonly ICreateFoodLogUseCase _createFoodLogUseCase;
    private readonly IGetFoodLogsByUserUseCase _getFoodLogsByUserUseCase;
    private readonly GetAllFoodLogsUseCase _getAllFoodLogsUseCase;
    private readonly UpdateFoodLogUseCase _updateFoodLogUseCase;
    private readonly DeleteFoodLogUseCase _deleteFoodLogUseCase;
    private readonly ILogger<FoodLogController> _logger;

    public FoodLogController(
        ICreateFoodLogUseCase createFoodLogUseCase,
        IGetFoodLogsByUserUseCase getFoodLogsByUserUseCase,
        GetAllFoodLogsUseCase getAllFoodLogsUseCase,
        UpdateFoodLogUseCase updateFoodLogUseCase,
        DeleteFoodLogUseCase deleteFoodLogUseCase,
        ILogger<FoodLogController> logger)
    {
        _createFoodLogUseCase = createFoodLogUseCase;
        _getFoodLogsByUserUseCase = getFoodLogsByUserUseCase;
        _getAllFoodLogsUseCase = getAllFoodLogsUseCase;
        _updateFoodLogUseCase = updateFoodLogUseCase;
        _deleteFoodLogUseCase = deleteFoodLogUseCase;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FoodLogResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _getAllFoodLogsUseCase.ExecuteAsync();
            var response = result.Select(FoodLogResponse.FromDto);
            
            return Ok(ApiResponse<IEnumerable<FoodLogResponse>>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all food logs");
            return StatusCode(ex.GetStatusCode(), ex.ToErrorResponse(includeStackTrace: false));
        }
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FoodLogResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByUser(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetFoodLogsByUserQuery(userId);
            var result = await _getFoodLogsByUserUseCase.ExecuteAsync(query, cancellationToken);
            var response = result.Select(FoodLogResponse.FromDto);
            
            return Ok(ApiResponse<IEnumerable<FoodLogResponse>>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving food logs for user {UserId}", userId);
            return StatusCode(ex.GetStatusCode(), ex.ToErrorResponse(includeStackTrace: false));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<FoodLogResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateFoodLogRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var command = request.ToCommand();
            var result = await _createFoodLogUseCase.ExecuteAsync(command, cancellationToken);
            var response = FoodLogResponse.FromDto(result);
            
            return CreatedAtAction(
                nameof(GetByUser), 
                new { userId = result.UserId }, 
                ApiResponse<FoodLogResponse>.SuccessResult(response, "Food log created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating food log");
            return StatusCode(ex.GetStatusCode(), ex.ToErrorResponse(includeStackTrace: false));
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<FoodLogResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateFoodLogRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var foodItems = request.FoodItems.Select(fi => new FoodItemDto(
                fi.FoodNutritionId,
                fi.Unit
            )).ToList();

            var result = await _updateFoodLogUseCase.ExecuteAsync(id, request.DateTime, foodItems);
            var response = FoodLogResponse.FromDto(result);
            
            return Ok(ApiResponse<FoodLogResponse>.SuccessResult(response, "Food log updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Food log with ID {FoodLogId} not found", id);
            return NotFound(ex.ToErrorResponse(includeStackTrace: false));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating food log {FoodLogId}", id);
            return StatusCode(ex.GetStatusCode(), ex.ToErrorResponse(includeStackTrace: false));
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _deleteFoodLogUseCase.ExecuteAsync(id);
            
            return Ok(ApiResponse<string>.SuccessResult("Food log deleted successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Food log with ID {FoodLogId} not found", id);
            return NotFound(ex.ToErrorResponse(includeStackTrace: false));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting food log {FoodLogId}", id);
            return StatusCode(ex.GetStatusCode(), ex.ToErrorResponse(includeStackTrace: false));
        }
    }
}
