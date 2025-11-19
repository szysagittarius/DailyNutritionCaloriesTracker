using Microsoft.EntityFrameworkCore;
using NT.Database.Entities;


namespace NT.Database.Context;

public class NutritionTrackerDbContext(DbContextOptions<NutritionTrackerDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<FoodLog> FoodLogs { get; set; }
    public DbSet<FoodItem> FoodItems { get; set; }
    public DbSet<FoodNutrition> FoodNutritions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("Nutrition");

        //Define table names and relationships explicitly if needed
        //modelBuilder.Entity<User>();

        //modelBuilder.Entity<FoodLog>()
        //    .HasOne(fl => fl.User)
        //    .WithMany()
        //    .HasForeignKey(fl => fl.UserId);

        //modelBuilder.Entity<FoodItem>()
        //    .HasOne(fi => fi.FoodLog)
        //    .WithMany()
        //    .HasForeignKey(fi => fi.FoodLogId);

        //modelBuilder.Entity<FoodItem>()
        //    .HasOne(fi => fi.FoodNutrition)
        //    .WithMany()
        //    .HasForeignKey(fi => fi.FoodNutritionId);

        //modelBuilder.Entity<FoodNutrition>().ToTable("FoodNutritions");

        // Additional configuration can go here
    }
}