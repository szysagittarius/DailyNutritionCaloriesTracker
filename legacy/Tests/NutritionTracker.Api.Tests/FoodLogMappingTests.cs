using AutoMapper;
using NT.Application.Contracts.Entities;
using NutritionTracker.Api.Models;
using NutritionTracker.Api.Profilers;

namespace NutritionTracker.Api.Tests;

public class FoodLogMappingTests
{
    private IMapper _mapper;
    [SetUp]
    public void Setup()
    {
        MapperConfiguration configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<FoodItemDtoProfiler>();
            cfg.AddProfile<FoodLogDtoProfiler>();
        });

        _mapper = configuration.CreateMapper();
    }
    [Test]
    public void DtoToEntityMapping_IsCorrect()
    {
        // Arrange
        FoodLogDto dto = new FoodLogDto
        {
            TotalCalories = 2000,
            TotalCarbs = 250,
            TotalProtein = 50,
            TotalFat = 70,
            UserId = Guid.NewGuid(),
            DateTime = DateTime.Now,
            CreateTime = DateTime.Now,
            UpdateTime = DateTime.Now,
            FoodItems = new List<FoodItemDto>
            {
                new FoodItemDto
                {
                    FoodNutritionId = Guid.NewGuid(),
                    Unit = 1,
                    FoodLogId = Guid.NewGuid()
                },
                new FoodItemDto
                {
                    FoodNutritionId = Guid.NewGuid(),
                    Unit = 2,
                    FoodLogId = Guid.NewGuid()
                }
            }.AsEnumerable()
        };

        // Act
        FoodLogEntity entity = _mapper.Map<FoodLogEntity>(dto);

        // Assert
        Assert.That(entity.TotalCalories, Is.EqualTo(dto.TotalCalories));
        Assert.That(entity.TotalCarbs, Is.EqualTo(dto.TotalCarbs));
        Assert.That(entity.TotalProtein, Is.EqualTo(dto.TotalProtein));
        Assert.That(entity.TotalFat, Is.EqualTo(dto.TotalFat));
        Assert.That(entity.UserId, Is.EqualTo(dto.UserId));
        Assert.That(entity.DateTime, Is.EqualTo(dto.DateTime));
        Assert.That(entity.CreateTime, Is.EqualTo(dto.CreateTime));
        Assert.That(entity.UpdateTime, Is.EqualTo(dto.UpdateTime));
        Assert.That(entity.FoodItems.Count(), Is.EqualTo(dto.FoodItems.Count()));
    }

    [Test]
    public void EntityToDtoMapping_IsCorrect()
    {
        // Arrange
        FoodLogEntity entity = new FoodLogEntity
        {
            TotalCalories = 2000,
            TotalCarbs = 250,
            TotalProtein = 50,
            TotalFat = 70,
            UserId = Guid.NewGuid(),
            DateTime = DateTime.Now,
            CreateTime = DateTime.Now,
            UpdateTime = DateTime.Now,
            FoodItems = new List<FoodItemEntity>
            {
                new FoodItemEntity
                {
                    FoodNutritionId = Guid.NewGuid(),
                    Unit = 1,
                    FoodLogId = Guid.NewGuid()
                },
                new FoodItemEntity
                {
                    FoodNutritionId = Guid.NewGuid(),
                    Unit = 2,
                    FoodLogId = Guid.NewGuid()
                }
            }
        };

        // Act
        FoodLogDto dto = _mapper.Map<FoodLogDto>(entity);

        // Assert
        Assert.That(dto.TotalCalories, Is.EqualTo(entity.TotalCalories));
        Assert.That(dto.TotalCarbs, Is.EqualTo(entity.TotalCarbs));
        Assert.That(dto.TotalProtein, Is.EqualTo(entity.TotalProtein));
        Assert.That(dto.TotalFat, Is.EqualTo(entity.TotalFat));
        Assert.That(dto.UserId, Is.EqualTo(entity.UserId));
        Assert.That(dto.DateTime, Is.EqualTo(entity.DateTime));
        Assert.That(dto.CreateTime, Is.EqualTo(entity.CreateTime));
        Assert.That(dto.UpdateTime, Is.EqualTo(entity.UpdateTime));
        Assert.That(dto.FoodItems.Count(), Is.EqualTo(entity.FoodItems.Count()));
    }

}