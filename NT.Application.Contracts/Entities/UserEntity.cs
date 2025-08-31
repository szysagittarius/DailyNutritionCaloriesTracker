﻿namespace NT.Application.Contracts.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int SuggestedCalories { get; set; } = 2000;
}
