-- ============================================================
-- Seed: FoodNutritions (dbo schema — src hexagonal project)
-- Source: legacy/DailyNutritionCaloriesTracker.Server/App_data/food_nutritional_values.json
-- Run against the database after: dotnet ef database update
-- Idempotent: skips rows where Name already exists
-- ============================================================

USE [NutritionTracker];
GO

MERGE [dbo].[FoodNutritions] AS Target
USING (VALUES
    ('Egg',          'g/100g',  0.72,  9.51, 12.56, 143.48),
    ('Pork Floss',   'g/100g',  1.00, 20.00, 55.00, 505.00),
    ('Kale',         'g/100g',  8.75,  1.49,  4.28,  49.12),
    ('Sausage',      'g/100g',  1.43, 31.84, 19.27, 350.49),
    ('Blueberry',    'g/100g', 14.49,  0.33,  0.74,  57.96),
    ('Carrot',       'g/100g',  9.58,  0.24,  0.93,  41.12),
    ('Walnut',       'g/100g', 13.71, 65.21, 15.23, 654.25),
    ('Chia Seeds',   'g/100g', 42.12, 30.74, 16.54, 486.70),
    ('Beet',         'g/100g',  9.56,  0.17,  1.61,  43.06),
    ('Celery',       'g/100g',  2.97,  0.17,  0.69,  16.25),
    ('Almond',       'g/100g', 21.55, 49.93, 21.15, 575.49),
    ('Salmon',       'g/100g',  0.00, 13.42, 20.42, 208.22),
    ('Crown Daisy',  'g/100g',  4.10,  0.70,  1.50,  29.60),
    ('Minced Meat',  'g/100g',  0.00, 20.00, 14.00, 260.00),
    ('Eggplant',     'g/100g',  5.88,  0.18,  0.98,  24.66)
) AS Source ([Name], [Measurement], [Carbs], [Fat], [Protein], [Calories])
ON Target.[Name] = Source.[Name]
WHEN NOT MATCHED THEN
    INSERT ([Id], [Name], [Measurement], [Carbs], [Fat], [Protein], [Calories])
    VALUES (NEWID(), Source.[Name], Source.[Measurement], Source.[Carbs], Source.[Fat], Source.[Protein], Source.[Calories]);

SELECT COUNT(*) AS [Total FoodNutritions after seed] FROM [dbo].[FoodNutritions];
GO
