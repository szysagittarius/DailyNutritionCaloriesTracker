-- Complete Database Setup Script
-- This script sets up the required data for the Nutrition Tracker application
-- Run this script to resolve foreign key constraint issues

-- Use the Nutrition schema
USE [NutritionTracker]; -- Replace with your actual database name
GO

PRINT 'Starting database setup...';
GO

-- ========================================
-- STEP 1: Insert Default User
-- ========================================
PRINT 'Step 1: Inserting default user...';

MERGE [Nutrition].[Users] AS Target
USING (VALUES
    ('00000000-0000-0000-0000-000000000001', 'Default User', 'default@nutrition.app', 'temp_password_hash')
) AS Source ([Id], [Name], [Email], [Password])
ON Target.[Email] = Source.[Email]
WHEN NOT MATCHED THEN
    INSERT ([Id], [Name], [Email], [Password])
    VALUES (Source.[Id], Source.[Name], Source.[Email], Source.[Password]);

PRINT 'Default user setup completed.';
GO

-- ========================================
-- STEP 2: Insert Food Nutrition Data
-- ========================================
PRINT 'Step 2: Inserting food nutrition data...';

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

PRINT 'Food nutrition data setup completed.';
GO

-- ========================================
-- VERIFICATION QUERIES
-- ========================================
PRINT 'Step 3: Verification...';

PRINT 'User Count:';
SELECT COUNT(*) as 'Total Users' FROM [Nutrition].[Users];

PRINT 'Food Nutrition Count:';
SELECT COUNT(*) as 'Total Food Nutrition Records' FROM [Nutrition].[FoodNutritions];

PRINT 'Sample Food Nutrition Data:';
SELECT TOP 5
    [Id],
    [Name],
    [Measurement],
    [Carbs],
    [Fat],
    [Protein],
    [Calories]
FROM [Nutrition].[FoodNutritions]
ORDER BY [Name];

PRINT '========================================';
PRINT 'Database setup completed successfully!';
PRINT '========================================';
PRINT 'IMPORTANT NOTES:';
PRINT '1. Default user is for development only - implement proper authentication before production';
PRINT '2. Your createfoodlog API should now work without foreign key constraint errors';
PRINT '3. The API will automatically use available food nutrition records';
GO
