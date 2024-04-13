﻿using NutritionTracker.Api.Models;
public class FoodLogDto : DtoBase
{
    public new Guid Id { get; private set; }
    public DateTime DateTime { get; private set; }
    public DateTime CreateTime { get; private set; }
    public DateTime UpdateTime { get; private set; }
    public Guid UserId { get; private set; }
    public UserDto User { get; private set; } // Assuming User is also refactored to a domain model

    public FoodLogDto(Guid id, DateTime dateTime, Guid userId, UserDto user)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        }

        Id = id;
        DateTime = dateTime;
        CreateTime = DateTime.Now;
        UpdateTime = DateTime.Now;
        UserId = userId;
        User = user ?? throw new ArgumentNullException(nameof(user), "User cannot be null.");
    }

    public void UpdateLog(DateTime dateTime)
    {
        DateTime = dateTime;
        UpdateTime = DateTime.Now;
    }

    // Additional methods to manipulate and query the food log
}