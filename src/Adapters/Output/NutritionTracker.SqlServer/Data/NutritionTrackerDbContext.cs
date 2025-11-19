using Microsoft.EntityFrameworkCore;
using NutritionTracker.SqlServer.Entities;

namespace NutritionTracker.SqlServer.Data;

public class NutritionTrackerDbContext : DbContext
{
    public NutritionTrackerDbContext(DbContextOptions<NutritionTrackerDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<FoodLogEntity> FoodLogs { get; set; }
    public DbSet<FoodItemEntity> FoodItems { get; set; }
    public DbSet<FoodNutritionEntity> FoodNutritions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<UserEntity>()
            .HasMany(u => u.FoodLogs)
            .WithOne(fl => fl.User)
            .HasForeignKey(fl => fl.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // FoodLog configuration
        modelBuilder.Entity<FoodLogEntity>()
            .HasMany(fl => fl.FoodItems)
            .WithOne(fi => fi.FoodLog)
            .HasForeignKey(fi => fi.FoodLogId)
            .OnDelete(DeleteBehavior.Cascade);

        // FoodNutrition configuration
        modelBuilder.Entity<FoodNutritionEntity>()
            .HasMany(fn => fn.FoodItems)
            .WithOne(fi => fi.FoodNutrition)
            .HasForeignKey(fi => fi.FoodNutritionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
