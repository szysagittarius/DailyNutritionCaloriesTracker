using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NT.Application.Contracts.Entities;
using NT.Application.Services.Abstractions;
using NutritionTracker.Api.Models;
using NutritionTracker.Api.Profilers;

namespace DailyNutritionCaloriesTracker.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class FoodLogController : ControllerBase
{
    private readonly ILogger<FoodLogController> _logger;
    private readonly IFoodLogService _foodLogService;
    private readonly IMapper _mapper;

    public FoodLogController(ILogger<FoodLogController> logger,
        IFoodLogService foodLogService,
        IMapper mapper)
    {
        _logger = logger;
        _foodLogService = foodLogService;
        _mapper = mapper;
    }

    [HttpGet("GetFoodLogs")]
    public async Task<IEnumerable<FoodLogDto>> Get() // Make this async
    {
        MapperConfiguration dtoMapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new FoodNutritionDtoProfiler());
            cfg.AddProfile(new FoodItemDtoProfiler());
            cfg.AddProfile(new FoodLogDtoProfiler());
            cfg.AddProfile(new UserDtoProfiler());
        });

        IMapper dtoMapper = dtoMapperConfig.CreateMapper();

        // Fix: Use await instead of .Result
        IEnumerable<FoodLogEntity> entities = await _foodLogService.GetAllAsync();

        IEnumerable<FoodLogDto> foodLogDtos = entities.Select(e => dtoMapper.Map<FoodLogDto>(e));

        return foodLogDtos;
    }

    // Updated endpoint to accept userId parameter and calculate totals from FoodItems
    [HttpGet("GetUserFoodLogs/{userId}")]
    public async Task<ActionResult<IEnumerable<FoodLogDto>>> GetUserFoodLogs(Guid userId)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("Valid user ID is required");
            }

            MapperConfiguration dtoMapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new FoodNutritionDtoProfiler());
                cfg.AddProfile(new FoodItemDtoProfiler());
                cfg.AddProfile(new FoodLogDtoProfiler());
                cfg.AddProfile(new UserDtoProfiler());
            });

            IMapper dtoMapper = dtoMapperConfig.CreateMapper();

            // Get food logs for the specific user - FoodItems will now be included
            IEnumerable<FoodLogEntity> entities = await _foodLogService.GetAllAsync();
            
            // Filter by user ID
            var userEntities = entities.Where(e => e.UserId == userId);

            IEnumerable<FoodLogDto> foodLogDtos = userEntities.Select(entity => {
                var dto = dtoMapper.Map<FoodLogDto>(entity);
                
                // Calculate totals from FoodItems using their FoodNutrition and Unit
                if (entity.FoodItems != null && entity.FoodItems.Any())
                {
                    dto.TotalCalories = entity.FoodItems.Sum(item => 
                        item.FoodNutrition != null ? (item.FoodNutrition.Calories * item.Unit / 100.0) : 0);
                    
                    dto.TotalCarbs = entity.FoodItems.Sum(item => 
                        item.FoodNutrition != null ? (item.FoodNutrition.Carbs * item.Unit / 100.0) : 0);
                    
                    dto.TotalProtein = entity.FoodItems.Sum(item => 
                        item.FoodNutrition != null ? (item.FoodNutrition.Protein * item.Unit / 100.0) : 0);
                    
                    dto.TotalFat = entity.FoodItems.Sum(item => 
                        item.FoodNutrition != null ? (item.FoodNutrition.Fat * item.Unit / 100.0) : 0);
                    
                    _logger.LogInformation("Calculated totals for FoodLog - Calories: {Calories}, Carbs: {Carbs}", 
                        dto.TotalCalories, dto.TotalCarbs);
                }
                else
                {
                    _logger.LogWarning("FoodLog has no FoodItems - keeping existing totals");
                }
                
                return dto;
            });

            _logger.LogInformation("Retrieved {Count} food log entries for user {UserId}", foodLogDtos.Count(), userId);

            return Ok(foodLogDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching food logs for user {UserId}", userId);
            return StatusCode(500, "An error occurred while fetching food logs");
        }
    }

    [HttpPost("createfoodlog")]
    public async Task<IActionResult> PostAsync([FromBody] FoodLogDto foodLogDto)
    {
        try
        {
            // Add logging to see what we're receiving
            _logger.LogInformation("Received FoodLogDto: UserId={UserId}, FoodItems count={Count}", 
                foodLogDto?.UserId, foodLogDto?.FoodItems?.Count() ?? 0);

            // Validate the incoming data
            if (foodLogDto == null)
            {
                _logger.LogWarning("FoodLogDto is null");
                return BadRequest("FoodLog data is required");
            }

            // Log the actual UserId value for debugging
            _logger.LogInformation("UserId received: {UserId})", 
                foodLogDto.UserId);

            if (foodLogDto.UserId == Guid.Empty || foodLogDto.UserId == null)
            {
                _logger.LogWarning("Invalid UserId: {UserId}", foodLogDto.UserId);
                return BadRequest("Valid user ID is required");
            }

            if (foodLogDto.FoodItems == null || !foodLogDto.FoodItems.Any())
            {
                _logger.LogWarning("No food items provided");
                return BadRequest("At least one food item is required");
            }

            // Validate that all food items have valid FoodNutritionId
            foreach (var item in foodLogDto.FoodItems)
            {
                if (item.FoodNutritionId == Guid.Empty || item.FoodNutritionId == null)
                {
                    _logger.LogWarning("Invalid FoodNutritionId for item: {ItemName}", item.Id);
                    return BadRequest($"Valid FoodNutritionId is required for food item: {item.Id}");
                }
            }

            MapperConfiguration dtoMapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new FoodNutritionDtoProfiler());
                cfg.AddProfile(new FoodItemDtoProfiler());
                cfg.AddProfile(new FoodLogDtoProfiler());
                cfg.AddProfile(new UserDtoProfiler());
            });

            IMapper dtoMapper = dtoMapperConfig.CreateMapper();
            FoodLogEntity entity = dtoMapper.Map<FoodLogEntity>(foodLogDto);

            await _foodLogService.AddAsync(entity);

            _logger.LogInformation("Successfully created food log for user {UserId} with {ItemCount} items", 
                foodLogDto.UserId, foodLogDto.FoodItems.Count());

            return Ok(new { Message = "Food log created successfully", Id = entity.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating food log");
            return StatusCode(500, "An error occurred while creating the food log");
        }
    }
}
