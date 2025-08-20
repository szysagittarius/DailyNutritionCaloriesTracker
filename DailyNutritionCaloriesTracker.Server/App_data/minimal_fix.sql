-- MINIMAL FIX: Insert required records to resolve foreign key constraints
-- This is the simplest solution to make the createfoodlog endpoint work

USE [NutritionTracker]; -- Replace with your actual database name if different
GO

-- Insert ONE default user with the exact GUID used in the controller
IF NOT EXISTS (SELECT 1 FROM [Nutrition].[Users] WHERE [Id] = '00000000-0000-0000-0000-000000000001')
BEGIN
    INSERT INTO [Nutrition].[Users] ([Id], [Name], [Email], [Password])
    VALUES ('00000000-0000-0000-0000-000000000001', 'Default User', 'default@nutrition.app', 'temp_password');
    PRINT 'Default user inserted.';
END
ELSE
BEGIN
    PRINT 'Default user already exists.';
END

-- Insert ONE food nutrition record with the exact GUID used in the controller
IF NOT EXISTS (SELECT 1 FROM [Nutrition].[FoodNutritions] WHERE [Id] = '11111111-1111-1111-1111-111111111111')
BEGIN
    INSERT INTO [Nutrition].[FoodNutritions] ([Id], [Name], [Measurement], [Carbs], [Fat], [Protein], [Calories])
    VALUES ('11111111-1111-1111-1111-111111111111', 'Generic Food', 'g/100g', 10.0, 5.0, 8.0, 100.0);
    PRINT 'Default food nutrition record inserted.';
END
ELSE
BEGIN
    PRINT 'Default food nutrition record already exists.';
END

-- Optional: Insert the JSON food data (you can run this separately if needed)
-- Uncomment the section below if you want all the food data from your JSON file

/*
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
*/

PRINT 'Minimal fix complete. The createfoodlog endpoint should now work.';
GO
