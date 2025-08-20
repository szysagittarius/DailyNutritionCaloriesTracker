-- Insert Food Nutrition Data from JSON
-- This script inserts food nutrition data into the FoodNutritions table
-- Data is inserted only if a record with the same name doesn't already exist

-- Use the Nutrition schema
USE [NutritionTracker]; -- Replace with your actual database name
GO

-- Insert food nutrition data with "INSERT IF NOT EXISTS" logic
-- Using MERGE statement to avoid duplicates based on Name

MERGE [Nutrition].[FoodNutritions] AS Target
USING (VALUES
    (NEWID(), 'Egg', 'g/100g', 0.72, 9.51, 12.56, 143.48),
    (NEWID(), 'Pork Floss', 'g/100g', 1.0, 20.0, 55.0, 505.0),
    (NEWID(), 'Kale', 'g/100g', 8.75, 1.49, 4.28, 49.12),
    (NEWID(), 'Sausage', 'g/100g', 1.43, 31.84, 19.27, 350.49),
    (NEWID(), 'Blueberry', 'g/100g', 14.49, 0.33, 0.74, 57.96),
    (NEWID(), 'Carrot', 'g/100g', 9.58, 0.24, 0.93, 41.12),
    (NEWID(), 'Walnut', 'g/100g', 13.71, 65.21, 15.23, 654.25),
    (NEWID(), 'Chia Seeds', 'g/100g', 42.12, 30.74, 16.54, 486.7),
    (NEWID(), 'Beet', 'g/100g', 9.56, 0.17, 1.61, 43.06),
    (NEWID(), 'Celery', 'g/100g', 2.97, 0.17, 0.69, 16.25),
    (NEWID(), 'Almond', 'g/100g', 21.55, 49.93, 21.15, 575.49),
    (NEWID(), 'Salmon', 'g/100g', 0.0, 13.42, 20.42, 208.22),
    (NEWID(), 'Crown Daisy', 'g/100g', 4.1, 0.7, 1.5, 29.6),
    (NEWID(), 'Minced Meat', 'g/100g', 0.0, 20.0, 14.0, 260.0),
    (NEWID(), 'Eggplant', 'g/100g', 5.88, 0.18, 0.98, 24.66)
) AS Source ([Id], [Name], [Measurement], [Carbs], [Fat], [Protein], [Calories])
ON Target.[Name] = Source.[Name]
WHEN NOT MATCHED THEN
    INSERT ([Id], [Name], [Measurement], [Carbs], [Fat], [Protein], [Calories])
    VALUES (Source.[Id], Source.[Name], Source.[Measurement], Source.[Carbs], Source.[Fat], Source.[Protein], Source.[Calories]);

-- Display the results
SELECT COUNT(*) as 'Total Records Inserted/Updated' FROM [Nutrition].[FoodNutritions];

-- Display all records to verify
SELECT 
    [Id],
    [Name],
    [Measurement],
    [Carbs],
    [Fat],
    [Protein],
    [Calories]
FROM [Nutrition].[FoodNutritions]
ORDER BY [Name];

PRINT 'Food nutrition data has been successfully inserted/updated in the FoodNutritions table.';
GO
