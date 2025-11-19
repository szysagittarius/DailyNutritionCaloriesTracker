using NutritionTracker.Domain.Entities;
using NutritionTracker.Persistence.Contracts.Extensions;
using NutritionTracker.SqlServer.Entities;

namespace NutritionTracker.SqlServer.Mappers;

public static class EntityMapper
{
    public static User ToDomain(UserEntity entity)
    {
        var user = ReflectionExtensions.CreateInstance<User>();
        user.SetPrivateProperty(nameof(User.Id), entity.Id);
        user.SetPrivateProperty(nameof(User.Name), entity.Name);
        user.SetPrivateProperty(nameof(User.Email), entity.Email);
        user.SetPrivateProperty(nameof(User.Password), entity.Password);
        user.SetPrivateProperty(nameof(User.SuggestedCalories), entity.SuggestedCalories);
        user.SetPrivateProperty(nameof(User.SuggestedCarbs), entity.SuggestedCarbs);
        user.SetPrivateProperty(nameof(User.SuggestedFat), entity.SuggestedFat);
        user.SetPrivateProperty(nameof(User.SuggestedProtein), entity.SuggestedProtein);
        return user;
    }

    public static UserEntity ToEntity(User domain)
    {
        return new UserEntity
        {
            Id = domain.Id,
            Name = domain.Name,
            Email = domain.Email,
            Password = domain.Password,
            SuggestedCalories = domain.SuggestedCalories,
            SuggestedCarbs = domain.SuggestedCarbs,
            SuggestedFat = domain.SuggestedFat,
            SuggestedProtein = domain.SuggestedProtein
        };
    }

    public static FoodLog ToDomain(FoodLogEntity entity)
    {
        var foodLog = ReflectionExtensions.CreateInstance<FoodLog>();
        foodLog.SetPrivateProperty(nameof(FoodLog.Id), entity.Id);
        foodLog.SetPrivateProperty(nameof(FoodLog.DateTime), entity.DateTime);
        foodLog.SetPrivateProperty(nameof(FoodLog.CreateTime), entity.CreateTime);
        foodLog.SetPrivateProperty(nameof(FoodLog.UpdateTime), entity.UpdateTime);
        foodLog.SetPrivateProperty(nameof(FoodLog.UserId), entity.UserId);
        foodLog.SetPrivateProperty(nameof(FoodLog.TotalCalories), entity.TotalCalories);
        foodLog.SetPrivateProperty(nameof(FoodLog.TotalCarbs), entity.TotalCarbs);
        foodLog.SetPrivateProperty(nameof(FoodLog.TotalProtein), entity.TotalProtein);
        foodLog.SetPrivateProperty(nameof(FoodLog.TotalFat), entity.TotalFat);

        if (entity.FoodItems?.Any() == true)
        {
            var foodItems = entity.FoodItems.Select(fi => ToDomain(fi)).ToList();
            var foodItemsList = foodLog.GetPrivateField<List<FoodItem>>("_foodItems");
            foodItemsList!.Clear();
            foodItemsList.AddRange(foodItems);
        }

        return foodLog;
    }

    public static FoodLogEntity ToEntity(FoodLog domain)
    {
        return new FoodLogEntity
        {
            Id = domain.Id,
            DateTime = domain.DateTime,
            CreateTime = domain.CreateTime,
            UpdateTime = domain.UpdateTime,
            UserId = domain.UserId,
            TotalCalories = domain.TotalCalories,
            TotalCarbs = domain.TotalCarbs,
            TotalProtein = domain.TotalProtein,
            TotalFat = domain.TotalFat,
            FoodItems = domain.FoodItems.Select(ToEntity).ToList()
        };
    }

    public static FoodItem ToDomain(FoodItemEntity entity)
    {
        var foodNutrition = entity.FoodNutrition != null ? ToDomain(entity.FoodNutrition) : null;
        
        var foodItem = ReflectionExtensions.CreateInstance<FoodItem>();
        foodItem.SetPrivateProperty(nameof(FoodItem.Id), entity.Id);
        foodItem.SetPrivateProperty(nameof(FoodItem.FoodNutritionId), entity.FoodNutritionId);
        foodItem.SetPrivateProperty(nameof(FoodItem.Unit), entity.Unit);
        foodItem.SetPrivateProperty(nameof(FoodItem.FoodLogId), entity.FoodLogId);
        if (foodNutrition != null)
            foodItem.SetPrivateProperty(nameof(FoodItem.FoodNutrition), foodNutrition);
        
        return foodItem;
    }

    public static FoodItemEntity ToEntity(FoodItem domain)
    {
        return new FoodItemEntity
        {
            Id = domain.Id,
            FoodNutritionId = domain.FoodNutritionId,
            Unit = domain.Unit,
            FoodLogId = domain.FoodLogId
        };
    }

    public static FoodNutrition ToDomain(FoodNutritionEntity entity)
    {
        var foodNutrition = ReflectionExtensions.CreateInstance<FoodNutrition>();
        foodNutrition.SetPrivateProperty(nameof(FoodNutrition.Id), entity.Id);
        foodNutrition.SetPrivateProperty(nameof(FoodNutrition.Name), entity.Name);
        foodNutrition.SetPrivateProperty(nameof(FoodNutrition.Measurement), entity.Measurement);
        foodNutrition.SetPrivateProperty(nameof(FoodNutrition.Carbs), entity.Carbs);
        foodNutrition.SetPrivateProperty(nameof(FoodNutrition.Fat), entity.Fat);
        foodNutrition.SetPrivateProperty(nameof(FoodNutrition.Protein), entity.Protein);
        foodNutrition.SetPrivateProperty(nameof(FoodNutrition.Calories), entity.Calories);
        return foodNutrition;
    }

    public static FoodNutritionEntity ToEntity(FoodNutrition domain)
    {
        return new FoodNutritionEntity
        {
            Id = domain.Id,
            Name = domain.Name,
            Measurement = domain.Measurement,
            Carbs = domain.Carbs,
            Fat = domain.Fat,
            Protein = domain.Protein,
            Calories = domain.Calories
        };
    }
}
