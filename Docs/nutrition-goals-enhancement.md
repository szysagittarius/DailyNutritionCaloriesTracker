# Nutrition Goals Enhancement

## Overview
Enhanced the user profile system to support individual nutrition goals for calories, carbs, fat, and protein. Users can now set personalized targets for each macronutrient instead of relying on calculated percentages.

## Changes Made

### 1. Database Schema Updates

#### Database Entity (`User.cs`)
```csharp
// filepath: DNCT.Database/Entities/User.cs
[Table("Users")]
public class User
{
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    public Guid Id { get; set; }
    
    [Column("Name", TypeName = "nvarchar(max)")]
    public string Name { get; set; } = string.Empty;
    
    [Column("Email", TypeName = "nvarchar(max)")]
    public string Email { get; set; } = string.Empty;
    
    [Column("Password", TypeName = "nvarchar(max)")]
    public string Password { get; set; } = string.Empty;
    
    [Column("SuggestedCalories", TypeName = "int")]
    public int SuggestedCalories { get; set; } = 2000;
    
    // NEW FIELDS ADDED
    [Column("SuggestedCarbs", TypeName = "int")]
    public int SuggestedCarbs { get; set; } = 246;
    
    [Column("SuggestedFat", TypeName = "int")]
    public int SuggestedFat { get; set; } = 68;
    
    [Column("SuggestedProtein", TypeName = "int")]
    public int SuggestedProtein { get; set; } = 215;
    
    public virtual ICollection<FoodLog> FoodLogs { get; set; } = new List<FoodLog>();
}
```

#### Application Entity (`UserEntity.cs`)
```csharp
// filepath: NT.Application.Contracts/Entities/UserEntity.cs
public class UserEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public int SuggestedCalories { get; set; }
    public int SuggestedCarbs { get; set; }      // NEW
    public int SuggestedFat { get; set; }        // NEW
    public int SuggestedProtein { get; set; }    // NEW
    
    public virtual ICollection<FoodLogEntity> FoodLogs { get; set; }
}
```

### 2. API Layer Updates

#### Data Transfer Objects
```csharp
// filepath: NutritionTracker.Api/Models/UserDto.cs
public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public int SuggestedCalories { get; set; }
    public int SuggestedCarbs { get; set; }      // NEW
    public int SuggestedFat { get; set; }        // NEW
    public int SuggestedProtein { get; set; }    // NEW
}

// filepath: NutritionTracker.Api/Models/UpdateUserProfileDto.cs
public class UpdateUserProfileDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public int SuggestedCalories { get; set; }
    public int SuggestedCarbs { get; set; }      // NEW
    public int SuggestedFat { get; set; }        // NEW
    public int SuggestedProtein { get; set; }    // NEW
}
```

#### AutoMapper Profiles
```csharp
// filepath: NT.Repositories/Profiler/UserProfiler.cs
internal class UserProfiler : Profile
{
    public UserProfiler()
    {
        CreateMap<UserEntity, User>()
            .ForMember(dest => dest.SuggestedCalories, opt => opt.MapFrom(src => src.SuggestedCalories))
            .ForMember(dest => dest.SuggestedCarbs, opt => opt.MapFrom(src => src.SuggestedCarbs))         // NEW
            .ForMember(dest => dest.SuggestedFat, opt => opt.MapFrom(src => src.SuggestedFat))             // NEW
            .ForMember(dest => dest.SuggestedProtein, opt => opt.MapFrom(src => src.SuggestedProtein));    // NEW

        CreateMap<User, UserEntity>()
            .ForMember(dest => dest.SuggestedCalories, opt => opt.MapFrom(src => src.SuggestedCalories))
            .ForMember(dest => dest.SuggestedCarbs, opt => opt.MapFrom(src => src.SuggestedCarbs))         // NEW
            .ForMember(dest => dest.SuggestedFat, opt => opt.MapFrom(src => src.SuggestedFat))             // NEW
            .ForMember(dest => dest.SuggestedProtein, opt => opt.MapFrom(src => src.SuggestedProtein));    // NEW
    }
}
```

### 3. Frontend Updates

#### Profile Form (`MyProfile.vue`)
```vue
<!-- New form fields added for individual nutrition goals -->
<div class="form-grid">
  <div class="form-group">
    <label for="suggestedCarbs" class="form-label">Daily Carbs Goal</label>
    <div class="macro-input-container">
      <input
        id="suggestedCarbs"
        v-model.number="profile.suggestedCarbs"
        type="number"
        class="form-input macro-input"
      />
      <span class="macro-unit">g</span>
    </div>
  </div>
  
  <div class="form-group">
    <label for="suggestedProtein" class="form-label">Daily Protein Goal</label>
    <div class="macro-input-container">
      <input
        id="suggestedProtein"
        v-model.number="profile.suggestedProtein"
        type="number"
        class="form-input macro-input"
      />
      <span class="macro-unit">g</span>
    </div>
  </div>
</div>

<div class="form-group">
  <label for="suggestedFat" class="form-label">Daily Fat Goal</label>
  <div class="macro-input-container">
    <input
      id="suggestedFat"
      v-model.number="profile.suggestedFat"
      type="number"
      class="form-input macro-input"
    />
    <span class="macro-unit">g</span>
  </div>
</div>
```

#### Data Model Updates
```javascript
// Updated profile data structure
data() {
  return {
    profile: {
      name: '',
      email: '',
      password: '',
      suggestedCalories: 2456,
      suggestedCarbs: 246,      // NEW
      suggestedFat: 68,         // NEW
      suggestedProtein: 215     // NEW
    },
    // ...
  }
}

// Updated profile loading
async loadUserProfile() {
  // ...
  if (response.ok) {
    const userData = await response.json()
    this.profile.name = userData.name || currentUser.username || ''
    this.profile.email = userData.email || ''
    this.profile.suggestedCalories = userData.suggestedCalories || 2456
    this.profile.suggestedCarbs = userData.suggestedCarbs || 246          // NEW
    this.profile.suggestedFat = userData.suggestedFat || 68               // NEW
    this.profile.suggestedProtein = userData.suggestedProtein || 215      // NEW
  }
  // ...
}
```

### 4. Database Migration

#### Migration Command
```bash
# From DailyNutritionCaloriesTracker.Server directory
dotnet ef migrations add AddNutritionGoalsToUsers --project ..\DNCT.Database\NT.Ef.Database.csproj
dotnet ef database update --project ..\DNCT.Database\NT.Ef.Database.csproj
```

#### Migration Script
```sql
-- Add new nutrition goal columns
ALTER TABLE [Users] ADD [SuggestedCarbs] int NOT NULL DEFAULT 246;
ALTER TABLE [Users] ADD [SuggestedFat] int NOT NULL DEFAULT 68;  
ALTER TABLE [Users] ADD [SuggestedProtein] int NOT NULL DEFAULT 215;
```

### 5. Default Values

| Field | Default Value | Description |
|-------|---------------|-------------|
| `SuggestedCalories` | 2456 | Based on average daily caloric needs |
| `SuggestedCarbs` | 246g | ~45% of 2456 calories (4 cal/g) |
| `SuggestedProtein` | 215g | ~30% of 2456 calories (4 cal/g) |
| `SuggestedFat` | 68g | ~25% of 2456 calories (9 cal/g) |

### 6. Form Validation

**Removed validation constraints:**
- No min/max limits on nutrition input fields
- No step restrictions
- No required validation on nutrition goals
- Users can enter any positive numeric values

**Retained validation:**
- Personal information fields (name, email) still required
- Type validation (numeric inputs only)

## Integration Points

### TodayNutritionSummary Component
```vue
<!-- Component now receives individual nutrition goals as props -->
<TodayNutritionSummary 
  :today-totals="todaySummary"
  :suggested-calories="userProfile.suggestedCalories"
  :suggested-carbs="userProfile.suggestedCarbs"
  :suggested-fat="userProfile.suggestedFat"
  :suggested-protein="userProfile.suggestedProtein"
/>
```

### MyFoodLog Component Updates
```javascript
// Fetch user profile data to get nutrition goals
const fetchUserProfile = async () => {
  try {
    if (currentUser.value?.username) {
      const response = await fetch(`/user/profile/${currentUser.value.username}`)
      if (response.ok) {
        const profile = await response.json()
        userProfile.value.suggestedCalories = profile.suggestedCalories || 2456
        userProfile.value.suggestedCarbs = profile.suggestedCarbs || 246
        userProfile.value.suggestedFat = profile.suggestedFat || 68
        userProfile.value.suggestedProtein = profile.suggestedProtein || 215
      }
    }
  } catch (error) {
    console.error('Error fetching user profile:', error)
  }
}
```

## Benefits

1. **Personalized Goals**: Users can set individual nutrition targets based on their specific needs
2. **Flexible Tracking**: No longer constrained to percentage-based calculations
3. **Better User Experience**: Direct input of desired macro targets
4. **Persistent Storage**: Goals are saved and loaded from the database
5. **Backward Compatibility**: Existing users get sensible default values

## Future Enhancements

- Add goal calculation wizards based on user weight/activity level
- Implement goal templates (e.g., "Weight Loss", "Muscle Gain", "Maintenance")
- Add progress tracking over time
- Include micronutrient goals (vitamins, minerals)
- Add goal achievement notifications and analytics

## Files Modified

### Backend
- `DNCT.Database/Entities/User.cs`
- `NT.Application.Contracts/Entities/UserEntity.cs`
- `NutritionTracker.Api/Models/UserDto.cs`
- `NutritionTracker.Api/Models/UpdateUserProfileDto.cs`
- `NT.Repositories/Profiler/UserProfiler.cs`
- `NutritionTracker.Api/Profilers/UserDtoProfiler.cs` (pending)
- `DailyNutritionCaloriesTracker.Server/Controllers/UserController.cs` (pending)

### Frontend
- `dailynutritioncaloriestracker.client/src/components/MyProfile.vue`
- `dailynutritioncaloriestracker.client/src/components/MyFoodLog.vue` (pending)
- `dailynutritioncaloriestracker.client/src/components/TodayNutritionSummary.vue` (ready for integration)

### Database
- New migration: `AddNutritionGoalsToUsers`
- Updated Users