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
    public IEnumerable<FoodLogDto> Get()
    {
        MapperConfiguration dtoMapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new FoodNutritionDtoProfiler());
            cfg.AddProfile(new FoodItemDtoProfiler());
            cfg.AddProfile(new FoodLogDtoProfiler());
            cfg.AddProfile(new UserDtoProfiler());
        });

        IMapper dtoMapper = dtoMapperConfig.CreateMapper();

        //load FoodLogDto from database by calling the service
        Task<IEnumerable<FoodLogEntity>> entities = _foodLogService.GetAllAsync();


        IEnumerable<FoodLogDto> foodLogDtos = entities.Result.Select(e => dtoMapper.Map<FoodLogDto>(e));


        return foodLogDtos;
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

        //2 need to fix bug here, on
        //FoodLogEntity entity = _mapper.Map<FoodLogEntity>(foodLogDto);

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
