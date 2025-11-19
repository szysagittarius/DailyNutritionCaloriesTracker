# Database Setup Instructions

This folder contains SQL scripts to set up the required data for the Nutrition Tracker application.

## Scripts Overview

### 1. `complete_database_setup.sql` (RECOMMENDED)
This is the main script that sets up everything you need:
- Inserts a default user to resolve foreign key constraints
- Inserts all food nutrition data from the JSON file
- Includes verification queries

### 2. `insert_food_nutrition_data.sql`
Standalone script that only inserts food nutrition data from the JSON file.

### 3. `insert_default_user.sql`
Standalone script that only inserts the default user.

## How to Use

### Quick Setup (Recommended)
1. Open SQL Server Management Studio (SSMS) or your preferred SQL client
2. Connect to your SQL Server instance
3. Open `complete_database_setup.sql`
4. Update the database name in the first line if different from `NutritionTracker`
5. Execute the script
6. Verify the output shows successful insertion

### Manual Step-by-Step
If you prefer to run scripts individually:
1. Run `insert_default_user.sql` first
2. Run `insert_food_nutrition_data.sql` second

## What This Fixes

The original error was:
```
The INSERT statement conflicted with the FOREIGN KEY constraint "FK_FoodLogs_Users_UserId"
The INSERT statement conflicted with the FOREIGN KEY constraint "FK_FoodItems_FoodLogs_FoodLogId"
```

This happened because:
- The hardcoded `UserId` didn't exist in the Users table
- The hardcoded `FoodNutritionId` didn't exist in the FoodNutritions table

## After Running the Scripts

Your `createfoodlog` API endpoint should now work properly because:
- A default user exists with ID `00000000-0000-0000-0000-000000000001`
- Food nutrition data is available in the database
- The controller automatically validates and uses available food nutrition records

## API Testing

You can test the endpoint with a request like:
```json
{
  "totalCalories": 200,
  "totalCarbs": 10,
  "totalProtein": 15,
  "totalFat": 8,
  "userId": "00000000-0000-0000-0000-000000000001",
  "dateTime": "2025-08-20T10:00:00Z",
  "createTime": "2025-08-20T10:00:00Z",
  "updateTime": "2025-08-20T10:00:00Z",
  "foodItems": [
    {
      "foodNutritionId": "00000000-0000-0000-0000-000000000000",
      "unit": 100,
      "foodLogId": "00000000-0000-0000-0000-000000000000"
    }
  ]
}
```

Note: The controller will automatically:
- Use the default user if userId is empty
- Find and use available food nutrition IDs if invalid ones are provided
- Set proper timestamps

## Important Security Notes

⚠️ **WARNING**: The default user setup is for DEVELOPMENT ONLY!

Before production:
1. Remove or disable the default user
2. Implement proper user authentication
3. Implement proper authorization
4. Hash passwords properly
5. Validate all user inputs

## Verification Queries

To check if data was inserted correctly:

```sql
-- Check users
SELECT * FROM [Nutrition].[Users];

-- Check food nutrition data
SELECT * FROM [Nutrition].[FoodNutritions];

-- Check food logs (after API testing)
SELECT * FROM [Nutrition].[FoodLogs];

-- Check food items (after API testing)  
SELECT * FROM [Nutrition].[FoodItems];
```
