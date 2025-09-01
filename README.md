# Nutrition Calories Tracker Project

## Project Overview
A web application inspired by the Microsoft To-Do app that helps users track their daily food intake and manage their nutritional goals.

## Features

### Backend
- **User Management**: Authentication, profile management using Azure AD.
- **Daily Caloric Intake Management**: Users can set and track against a daily caloric limit. Calory can be set from user profile, based on weight,height etc
- **Nutrition/Food Dictionary**: A database of foods and their nutritional information.
- **Data Analysis and Summary Tracing**: Tools for tracking and analyzing dietary patterns over time.
- **ChatGPT integation**: for consulting the daily food nutrition limit based on MBI etc
- **add admin user role**: later to add food items to the database
  
### UI
- **Auto-complete Food Entry**: Streamline food logging with an auto-complete feature.
- **Daily Summary and Progress Tracking**: Visual summaries of daily intake and goal progression.
- **Responsive and Accessible Design**: Ensure broad access and usability.

## Technology Stack
- Backend: .NET, Azure SQL Database/AWS RDS, Azure AD for authentication
- Frontend: HTML5, CSS3, JavaScript (AJAX for dynamic data fetching)
- Data Analysis: Potential future integration with data mining tools


## Data source
- from USDA National Nutrient Database - "https://fdc.nal.usda.gov/data-documentation.html"


## Database update
- To update the database,
- Need to install the EF Core design package in both the server and database projects.
- run the following command in the Package Manager Console:
```
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design //this will update the package, and mass up the version, dont do it


need to update appsetting.development.json,

just need to clean up the migration folder under Database, then run the command below, or you can ignore
cd DailyNutritionCaloriesTracker.Server
DailyNutritionCaloriesTracker\DailyNutritionCaloriesTracker.Server> dotnet ef migrations add InitialCreate --project ..\DNCT.Database\NT.Ef.Database.csproj

in future update, once there is db update, run sth like
cd DailyNutritionCaloriesTracker.Server
dotnet ef migrations add AddNutritionGoalsToUsers --project ..\DNCT.Database\NT.Ef.Database.csproj


lastly, 
 then run this command
cd DailyNutritionCaloriesTracker.Server
DailyNutritionCaloriesTracker\DailyNutritionCaloriesTracker.Server> dotnet ef database update --project ..\DNCT.Database\NT.Ef.Database.csproj
```