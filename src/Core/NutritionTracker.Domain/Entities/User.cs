namespace NutritionTracker.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public Guid RoleId { get; private set; }
    public string RoleName => RoleId == UserRole.DefaultAdminRoleId ? UserRole.AdminRoleName : UserRole.UserRoleName;
    public bool IsAdmin => RoleId == UserRole.DefaultAdminRoleId;
    public double SuggestedCalories { get; private set; }
    public double SuggestedCarbs { get; private set; }
    public double SuggestedFat { get; private set; }
    public double SuggestedProtein { get; private set; }
    
    private readonly List<FoodLog> _foodLogs = new();
    public IReadOnlyCollection<FoodLog> FoodLogs => _foodLogs.AsReadOnly();

    private User() { } // For EF Core

    public User(Guid id, string name, string email, string password,
        double suggestedCalories = 2000, double suggestedCarbs = 246,
        double suggestedFat = 68, double suggestedProtein = 215, Guid? roleId = null)
    {
        if (string.IsNullOrWhiteSpace(name)) 
            throw new ArgumentException("Name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(email)) 
            throw new ArgumentException("Email cannot be empty", nameof(email));
        if (string.IsNullOrWhiteSpace(password)) 
            throw new ArgumentException("Password cannot be empty", nameof(password));

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Name = name;
        Email = email;
        Password = password;
        RoleId = roleId ?? UserRole.DefaultUserRoleId;
        SuggestedCalories = suggestedCalories;
        SuggestedCarbs = suggestedCarbs;
        SuggestedFat = suggestedFat;
        SuggestedProtein = suggestedProtein;
    }

    public static User Create(string name, string email, string password,
        double suggestedCalories = 2000, double suggestedCarbs = 246,
        double suggestedFat = 68, double suggestedProtein = 215, Guid? roleId = null)
    {
        return new User(Guid.NewGuid(), name, email, password,
            suggestedCalories, suggestedCarbs, suggestedFat, suggestedProtein, roleId ?? UserRole.DefaultUserRoleId);
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        Name = name;
    }

    public void UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        Email = email;
    }

    public void UpdatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));
        Password = password;
    }

    public void UpdateRole(Guid roleId)
    {
        if (roleId == Guid.Empty)
            throw new ArgumentException("Role id cannot be empty", nameof(roleId));

        RoleId = roleId;
    }

    public void UpdateNutritionalGoals(double suggestedCalories, double suggestedCarbs,
        double suggestedFat, double suggestedProtein)
    {
        SuggestedCalories = suggestedCalories;
        SuggestedCarbs = suggestedCarbs;
        SuggestedFat = suggestedFat;
        SuggestedProtein = suggestedProtein;
    }

    public void UpdateProfile(string name, double suggestedCalories, double suggestedCarbs, 
        double suggestedFat, double suggestedProtein)
    {
        if (string.IsNullOrWhiteSpace(name)) 
            throw new ArgumentException("Name cannot be empty", nameof(name));

        Name = name;
        SuggestedCalories = suggestedCalories;
        SuggestedCarbs = suggestedCarbs;
        SuggestedFat = suggestedFat;
        SuggestedProtein = suggestedProtein;
    }

    public void AddFoodLog(FoodLog foodLog)
    {
        _foodLogs.Add(foodLog);
    }
}
