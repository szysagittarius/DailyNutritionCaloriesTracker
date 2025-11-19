using System.Text.Json;
using NutritionTracker.AzureTableStorage.Entities;
using NutritionTracker.Domain.Entities;
using NutritionTracker.Persistence.Contracts.Extensions;

namespace NutritionTracker.AzureTableStorage.Mappers;

public static class TableEntityMapper
{
    // User mappings
    public static UserTableEntity ToTableEntity(User domain)
    {
        return new UserTableEntity
        {
            PartitionKey = "USER",
            RowKey = domain.Id.ToString(),
            Name = domain.Name,
            Email = domain.Email,
            Password = domain.Password,
            SuggestedCalories = domain.SuggestedCalories,
            SuggestedCarbs = domain.SuggestedCarbs,
            SuggestedFat = domain.SuggestedFat,
            SuggestedProtein = domain.SuggestedProtein
        };
    }

    public static User ToDomain(UserTableEntity entity)
    {
        var user = ReflectionExtensions.CreateInstance<User>();
        user.SetPrivateProperty(nameof(User.Id), Guid.Parse(entity.RowKey));
        user.SetPrivateProperty(nameof(User.Name), entity.Name);
        user.SetPrivateProperty(nameof(User.Email), entity.Email);
        user.SetPrivateProperty(nameof(User.Password), entity.Password);
        user.SetPrivateProperty(nameof(User.SuggestedCalories), entity.SuggestedCalories);
        user.SetPrivateProperty(nameof(User.SuggestedCarbs), entity.SuggestedCarbs);
        user.SetPrivateProperty(nameof(User.SuggestedFat), entity.SuggestedFat);
        user.SetPrivateProperty(nameof(User.SuggestedProtein), entity.SuggestedProtein);
        return user;
    }

    // FoodNutrition mappings
    public static FoodNutritionTableEntity ToTableEntity(FoodNutrition domain)
    {
        return new FoodNutritionTableEntity
        {
            PartitionKey = "FOOD",
            RowKey = domain.Id.ToString(),
            Name = domain.Name,
            Measurement = domain.Measurement,
            Carbs = domain.Carbs,
            Fat = domain.Fat,
            Protein = domain.Protein,
            Calories = domain.Calories
        };
    }

    public static FoodNutrition ToDomain(FoodNutritionTableEntity entity)
    {
        var foodNutrition = ReflectionExtensions.CreateInstance<FoodNutrition>();
        foodNutrition.SetPrivateProperty(nameof(FoodNutrition.Id), Guid.Parse(entity.RowKey));
        foodNutrition.SetPrivateProperty(nameof(FoodNutrition.Name), entity.Name);
        foodNutrition.SetPrivateProperty(nameof(FoodNutrition.Measurement), entity.Measurement);
        foodNutrition.SetPrivateProperty(nameof(FoodNutrition.Carbs), entity.Carbs);
        foodNutrition.SetPrivateProperty(nameof(FoodNutrition.Fat), entity.Fat);
        foodNutrition.SetPrivateProperty(nameof(FoodNutrition.Protein), entity.Protein);
        foodNutrition.SetPrivateProperty(nameof(FoodNutrition.Calories), entity.Calories);
        return foodNutrition;
    }

    // FoodLog mappings
    public static FoodLogTableEntity ToTableEntity(FoodLog domain)
    {
        // Serialize food items with denormalized nutrition data
        var foodItemsData = domain.FoodItems.Select(fi => new FoodItemData
        {
            Id = fi.Id.ToString(),
            FoodNutritionId = fi.FoodNutritionId.ToString(),
            Unit = fi.Unit,
            FoodLogId = fi.FoodLogId.ToString(),
            FoodName = fi.FoodNutrition?.Name ?? "",
            Measurement = fi.FoodNutrition?.Measurement ?? "",
            Carbs = fi.FoodNutrition?.Carbs ?? 0,
            Fat = fi.FoodNutrition?.Fat ?? 0,
            Protein = fi.FoodNutrition?.Protein ?? 0,
            Calories = fi.FoodNutrition?.Calories ?? 0
        }).ToList();

        // Create sortable RowKey: YYYYMMDDHHMMSS_LogId
        var rowKey = $"{domain.DateTime:yyyyMMddHHmmss}_{domain.Id}";

        return new FoodLogTableEntity
        {
            PartitionKey = domain.UserId.ToString(),
            RowKey = rowKey,
            Id = domain.Id.ToString(),
            DateTime = domain.DateTime,
            CreateTime = domain.CreateTime,
            UpdateTime = domain.UpdateTime,
            UserId = domain.UserId.ToString(),
            TotalCalories = domain.TotalCalories,
            TotalCarbs = domain.TotalCarbs,
            TotalProtein = domain.TotalProtein,
            TotalFat = domain.TotalFat,
            FoodItemsJson = JsonSerializer.Serialize(foodItemsData)
        };
    }

    public static FoodLog ToDomain(FoodLogTableEntity entity)
    {
        var foodLog = ReflectionExtensions.CreateInstance<FoodLog>();
        foodLog.SetPrivateProperty(nameof(FoodLog.Id), Guid.Parse(entity.Id));
        foodLog.SetPrivateProperty(nameof(FoodLog.DateTime), entity.DateTime);
        foodLog.SetPrivateProperty(nameof(FoodLog.CreateTime), entity.CreateTime);
        foodLog.SetPrivateProperty(nameof(FoodLog.UpdateTime), entity.UpdateTime);
        foodLog.SetPrivateProperty(nameof(FoodLog.UserId), Guid.Parse(entity.UserId));
        foodLog.SetPrivateProperty(nameof(FoodLog.TotalCalories), entity.TotalCalories);
        foodLog.SetPrivateProperty(nameof(FoodLog.TotalCarbs), entity.TotalCarbs);
        foodLog.SetPrivateProperty(nameof(FoodLog.TotalProtein), entity.TotalProtein);
        foodLog.SetPrivateProperty(nameof(FoodLog.TotalFat), entity.TotalFat);

        // Deserialize food items
        if (!string.IsNullOrEmpty(entity.FoodItemsJson))
        {
            var foodItemsData = JsonSerializer.Deserialize<List<FoodItemData>>(entity.FoodItemsJson);
            if (foodItemsData?.Any() == true)
            {
                var foodItemsList = foodLog.GetPrivateField<List<FoodItem>>("_foodItems");
                foodItemsList!.Clear();

                foreach (var data in foodItemsData)
                {
                    // Reconstruct FoodNutrition
                    var foodNutrition = ReflectionExtensions.CreateInstance<FoodNutrition>();
                    foodNutrition.SetPrivateProperty(nameof(FoodNutrition.Id), Guid.Parse(data.FoodNutritionId));
                    foodNutrition.SetPrivateProperty(nameof(FoodNutrition.Name), data.FoodName);
                    foodNutrition.SetPrivateProperty(nameof(FoodNutrition.Measurement), data.Measurement);
                    foodNutrition.SetPrivateProperty(nameof(FoodNutrition.Carbs), data.Carbs);
                    foodNutrition.SetPrivateProperty(nameof(FoodNutrition.Fat), data.Fat);
                    foodNutrition.SetPrivateProperty(nameof(FoodNutrition.Protein), data.Protein);
                    foodNutrition.SetPrivateProperty(nameof(FoodNutrition.Calories), data.Calories);

                    // Reconstruct FoodItem
                    var foodItem = ReflectionExtensions.CreateInstance<FoodItem>();
                    foodItem.SetPrivateProperty(nameof(FoodItem.Id), Guid.Parse(data.Id));
                    foodItem.SetPrivateProperty(nameof(FoodItem.FoodNutritionId), Guid.Parse(data.FoodNutritionId));
                    foodItem.SetPrivateProperty(nameof(FoodItem.Unit), data.Unit);
                    foodItem.SetPrivateProperty(nameof(FoodItem.FoodLogId), Guid.Parse(data.FoodLogId));
                    foodItem.SetPrivateProperty(nameof(FoodItem.FoodNutrition), foodNutrition);

                    foodItemsList.Add(foodItem);
                }
            }
        }

        return foodLog;
    }
}
