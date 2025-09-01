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
        //insert the FoodLogDto to the database by calling the service

        //1
        MapperConfiguration dtoMapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new FoodNutritionDtoProfiler());
            cfg.AddProfile(new FoodItemDtoProfiler());
            cfg.AddProfile(new FoodLogDtoProfiler());
            cfg.AddProfile(new UserDtoProfiler());
        });

        IMapper dtoMapper = dtoMapperConfig.CreateMapper();

        FoodLogEntity entity3 = dtoMapper.Map<FoodLogEntity>(foodLogDto);

        //temp bypass, need to covert to real after user controller added
        entity3.UserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        //UI work need to include the guid, it will be removed
        foreach (FoodItemEntity item in entity3.FoodItems)
        {
            // Use the first food nutrition ID from the database (will be populated by SQL script)
            item.FoodNutritionId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        }

        // Process the data
        await _foodLogService.AddAsync(entity3);

        return Ok();
    }
}
