using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NT.Application.Contracts.Entities;
using NT.Application.Services.Abstractions;
using NutritionTracker.Api.Models;
using NutritionTracker.Api.Profilers;

namespace DailyNutritionCaloriesTracker.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class FoodNutritionController : ControllerBase
{

    private readonly ILogger<FoodNutritionController> _logger;
    private readonly IFoodNutritionService _foodNutritionService;
    private readonly IMapper _mapper;

    public FoodNutritionController(ILogger<FoodNutritionController> logger,
        IFoodNutritionService foodNutritionService,
        IMapper mapper)
    {
        _logger = logger;
        _foodNutritionService = foodNutritionService;
        _mapper = mapper;
    }

    // Add this new GET endpoint for nutrition management page
    [HttpGet]
    public async Task<IEnumerable<FoodNutritionDto>> GetAll()
    {
        MapperConfiguration dtoMapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new FoodNutritionDtoProfiler());
            cfg.AddProfile(new FoodItemDtoProfiler());
            cfg.AddProfile(new FoodLogDtoProfiler());
            cfg.AddProfile(new UserDtoProfiler());
        });

        IMapper dtoMapper = dtoMapperConfig.CreateMapper();

        IEnumerable<FoodNutritionEntity> entities = await _foodNutritionService.GetFoodNutritionAsync();

        IEnumerable<FoodNutritionDto> entitieDtos = entities.Select(e => dtoMapper.Map<FoodNutritionDto>(e));

        return entitieDtos;
    }

    // Add this new POST endpoint for adding food nutrition
    [HttpPost]
    public async Task<ActionResult<FoodNutritionDto>> Post([FromBody] FoodNutritionDto foodNutritionDto)
    {
        try
        {
            FoodNutritionEntity entity = MapDtoToEntity(foodNutritionDto);
            await _foodNutritionService.AddFoodNutritionAsync(entity);
            return Ok(foodNutritionDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding food nutrition");
            return StatusCode(500, "Error adding food nutrition");
        }
    }

    [HttpGet("getlist")]
    public async Task<IEnumerable<FoodNutritionDto>> Get()
    {
        MapperConfiguration dtoMapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new FoodNutritionDtoProfiler());
            cfg.AddProfile(new FoodItemDtoProfiler());
            cfg.AddProfile(new FoodLogDtoProfiler());
            cfg.AddProfile(new UserDtoProfiler());
        });

        IMapper dtoMapper = dtoMapperConfig.CreateMapper();

        IEnumerable<FoodNutritionEntity> entities = await _foodNutritionService.GetFoodNutritionAsync();

        IEnumerable<FoodNutritionDto> entitieDtos = entities.Select(e => dtoMapper.Map<FoodNutritionDto>(e));


        return entitieDtos;
    }

    //if this changes to public, it will mess up the swagger documentation
    private IEnumerable<FoodNutritionDto> LoadData()
    {
        //load data from json file, under App_data folder in the project
        string json = System.IO.File.ReadAllText("App_Data/food_nutritional_values.json");

        Dictionary<string, FoodNutritionDto>? foodNutritionDictionary = JsonConvert.DeserializeObject<Dictionary<string, FoodNutritionDto>>(json);

        List<FoodNutritionDto> foodNutritionList = foodNutritionDictionary.Select(kvp =>
        {
            FoodNutritionDto foodNutrition = kvp.Value;
            //foodNutrition.Id = Guid.NewGuid();
            foodNutrition.Name = kvp.Key;
            foodNutrition.Measurement = "per 100g";
            return foodNutrition;
        }).ToList();


        return foodNutritionList;
    }

    private FoodNutritionEntity MapDtoToEntity(FoodNutritionDto dto)
    {
        FoodNutritionEntity entity = new FoodNutritionEntity
        {
            // Assuming Id is generated elsewhere or not needed in this context
            Name = dto.Name,
            Measurement = dto.Measurement,
            Calories = dto.Calories,
            Protein = dto.Protein,
            Carbs = dto.Carbs,
            Fat = dto.Fat
        };

        return entity;
    }

    private FoodNutritionDto MapEntityToDto(FoodNutritionEntity entity)
    {
        FoodNutritionDto dto = new FoodNutritionDto
        {
            Name = entity.Name,
            Measurement = entity.Measurement,
            Calories = entity.Calories,
            Protein = entity.Protein,
            Carbs = entity.Carbs,
            Fat = entity.Fat
        };

        return dto;
    }

    private async Task AddFoodNutritionListAsync(List<FoodNutritionDto> foodNutritionList)
    {
        //Imapper map the list of FoodNutritionDto to list of FoodNutritionEntity
        List<FoodNutritionEntity> foodNutritionEntities = new List<FoodNutritionEntity>();

        try
        {
            foreach (FoodNutritionDto foodNutrition in foodNutritionList)
            {
                //FoodNutritionEntity foodNutritionEntity = _mapper.Map<FoodNutritionEntity>(foodNutrition);

                FoodNutritionEntity foodNutritionEntity = MapDtoToEntity(foodNutrition);
                foodNutritionEntities.Add(foodNutritionEntity);
            }
        }
        catch (Exception)
        {

            throw;
        }

        //add the list of FoodNutritionEntity to the database
        foreach (FoodNutritionEntity foodNutrition in foodNutritionEntities)
        {
            await _foodNutritionService.AddFoodNutritionAsync(foodNutrition);
        }
    }
}
