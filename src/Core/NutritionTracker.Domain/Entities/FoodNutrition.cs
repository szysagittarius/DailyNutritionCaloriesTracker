namespace NutritionTracker.Domain.Entities;

public class FoodNutrition
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Measurement { get; private set; }
    public double Carbs { get; private set; }
    public double Fat { get; private set; }
    public double Protein { get; private set; }
    public double Calories { get; private set; }

    private FoodNutrition() { } // For EF Core

    public FoodNutrition(Guid id, string name, string measurement, 
        double carbs, double fat, double protein, double calories)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(measurement))
            throw new ArgumentException("Measurement cannot be empty", nameof(measurement));

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Name = name;
        Measurement = measurement;
        Carbs = carbs;
        Fat = fat;
        Protein = protein;
        Calories = calories;
    }

    public static FoodNutrition Create(string name, string measurement, 
        double carbs, double fat, double protein, double calories)
    {
        return new FoodNutrition(Guid.NewGuid(), name, measurement, carbs, fat, protein, calories);
    }

    public void Update(string name, string measurement, double carbs, double fat, double protein, double calories)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(measurement))
            throw new ArgumentException("Measurement cannot be empty", nameof(measurement));

        Name = name;
        Measurement = measurement;
        Carbs = carbs;
        Fat = fat;
        Protein = protein;
        Calories = calories;
    }
}
