using AutoMapper;
using NT.Application.Contracts.Entities;
using NutritionTracker.Api.Models;
using NutritionTracker.Api.Profilers;

namespace NutritionTracker.Api.Tests;

public class FoodItemMappingTests
{
    private IMapper _mapper;
    [SetUp]
    public void Setup()
    {
        MapperConfiguration configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<FoodItemDtoProfiler>();
        });

        _mapper = configuration.CreateMapper();
    }

    [Test]
    public void DtoToEntityMapping_IsCorrect()
    {
        // Arrange
        FoodItemDto dto = new FoodItemDto
        {
            FoodNutritionId = Guid.NewGuid(),
            Unit = 100,
            FoodLogId = Guid.NewGuid()
        };

        // Act
        FoodItemEntity entity = _mapper.Map<FoodItemEntity>(dto);

        // Assert
        Assert.That(entity.FoodNutritionId, Is.EqualTo(dto.FoodNutritionId));
        Assert.That(entity.Unit, Is.EqualTo(dto.Unit));
        Assert.That(entity.FoodLogId, Is.EqualTo(dto.FoodLogId));
    }

    [Test]
    public void EntityToDtoMapping_IsCorrect()
    {
        // Arrange
        FoodItemEntity entity = new FoodItemEntity
        {
            FoodNutritionId = Guid.NewGuid(),
            Unit = 100,
            FoodLogId = Guid.NewGuid()
        };

        // Act
        FoodItemDto dto = _mapper.Map<FoodItemDto>(entity);

        // Assert
        Assert.That(dto.FoodNutritionId, Is.EqualTo(entity.FoodNutritionId));
        Assert.That(dto.Unit, Is.EqualTo(entity.Unit));
        Assert.That(dto.FoodLogId, Is.EqualTo(entity.FoodLogId));
    }
}