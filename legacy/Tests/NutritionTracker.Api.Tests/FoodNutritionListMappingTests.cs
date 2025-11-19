using AutoMapper;
using Newtonsoft.Json;
using NT.Application.Contracts.Entities;
using NutritionTracker.Api.Models;
using NutritionTracker.Api.Profilers;

namespace NutritionTracker.Api.Tests;

public class FoodNutritionListMappingTests
{
    private IMapper _mapper;
    [SetUp]
    public void Setup()
    {
        MapperConfiguration configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<FoodNutritionDtoProfiler>(); // Assuming your profile class is named FoodNutritionDtoProfiler
        });

        _mapper = configuration.CreateMapper();
    }

    [Test]
    public void EntityToDtoMapping_IsCorrect()
    {
        // Arrange
        FoodNutritionEntity entity = new FoodNutritionEntity
        {
            //Id = Guid.NewGuid(),
            Name = "Apple",
            Measurement = "100g",
            Calories = 52,
            Protein = 0.3,
            Carbs = 14,
            Fat = 0.2
        };

        // Act
        FoodNutritionDto dto = _mapper.Map<FoodNutritionDto>(entity);

        // Assert
        Assert.That(dto.Name, Is.EqualTo(entity.Name));
        Assert.That(dto.Measurement, Is.EqualTo(entity.Measurement));
        Assert.That(dto.Calories, Is.EqualTo(entity.Calories));
        Assert.That(dto.Protein, Is.EqualTo(entity.Protein));
        Assert.That(dto.Carbs, Is.EqualTo(entity.Carbs));
        Assert.That(dto.Fat, Is.EqualTo(entity.Fat));
    }

    [Test]
    public void DtoToEntityMapping_IsCorrect()
    {
        // Arrange
        FoodNutritionDto dto = new FoodNutritionDto
        {
            Name = "Banana",
            Measurement = "100g",
            Calories = 89,
            Protein = 1.1,
            Carbs = 23,
            Fat = 0.3
        };

        // Act
        FoodNutritionEntity entity = _mapper.Map<FoodNutritionEntity>(dto);

        // Assert
        Assert.That(entity.Name, Is.EqualTo(dto.Name));
        Assert.That(entity.Measurement, Is.EqualTo(dto.Measurement));
        Assert.That(entity.Calories, Is.EqualTo(dto.Calories));
        Assert.That(entity.Protein, Is.EqualTo(dto.Protein));
        Assert.That(entity.Carbs, Is.EqualTo(dto.Carbs));
        Assert.That(entity.Fat, Is.EqualTo(dto.Fat));
    }

    [Test]
    public void ListOfDtoToEntityMapping_IsCorrect()
    {
        // Arrange
        List<FoodNutritionDto> foodNutritionList = new List<FoodNutritionDto>
            {
                new FoodNutritionDto
                {
                    Name = "Apple",
                    Measurement = "100g",
                    Calories = 52,
                    Protein = 0.3,
                    Carbs = 14,
                    Fat = 0.2
                },
                new FoodNutritionDto
                {
                    Name = "Banana",
                    Measurement = "100g",
                    Calories = 89,
                    Protein = 1.1,
                    Carbs = 23,
                    Fat = 0.3
                }
            };

        // Act
        List<FoodNutritionEntity> foodNutritionEntities = _mapper.Map<List<FoodNutritionEntity>>(foodNutritionList);

        // Assert
        Assert.That(foodNutritionEntities.Count, Is.EqualTo(foodNutritionList.Count), "The mapped list has a different number of items.");

        for (int i = 0; i < foodNutritionList.Count; i++)
        {
            Assert.That(foodNutritionEntities[i].Name, Is.EqualTo(foodNutritionList[i].Name), $"Item {i} Name does not match.");
            Assert.That(foodNutritionEntities[i].Measurement, Is.EqualTo(foodNutritionList[i].Measurement), $"Item {i} Measurement does not match.");
            Assert.That(foodNutritionEntities[i].Calories, Is.EqualTo(foodNutritionList[i].Calories), $"Item {i} Calories does not match.");
            Assert.That(foodNutritionEntities[i].Protein, Is.EqualTo(foodNutritionList[i].Protein), $"Item {i} Protein does not match.");
            Assert.That(foodNutritionEntities[i].Carbs, Is.EqualTo(foodNutritionList[i].Carbs), $"Item {i} Carbs does not match.");
            Assert.That(foodNutritionEntities[i].Fat, Is.EqualTo(foodNutritionList[i].Fat), $"Item {i} Fat does not match.");
        }
    }

    [Test]
    public void ListOfDtoToEntityMappingFromFile_IsCorrect()
    {
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


        //Imapper map the list of FoodNutritionDto to list of FoodNutritionEntity
        List<FoodNutritionEntity> foodNutritionEntities = new List<FoodNutritionEntity>();

        foreach (FoodNutritionDto foodNutrition in foodNutritionList)
        {
            FoodNutritionEntity foodNutritionEntity = _mapper.Map<FoodNutritionEntity>(foodNutrition);
            foodNutritionEntities.Add(foodNutritionEntity);
        }

        Assert.That(foodNutritionEntities.Count, Is.EqualTo(15), $"Item counts is not 15.");

    }

}