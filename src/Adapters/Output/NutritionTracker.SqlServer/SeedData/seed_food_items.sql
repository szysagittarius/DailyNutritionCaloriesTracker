-- ============================================================
-- Seed: FoodItems (dbo schema - src hexagonal project)
-- Prerequisites: seed_food_nutrition.sql, seed_users.sql, seed_food_logs.sql
-- Run against the database after: dotnet ef database update
-- Idempotent: skips rows where FoodLogId + FoodNutritionId + Unit already exists
-- ============================================================

USE [NutritionTracker];
GO

WITH SourceItems AS (
    -- Demo User breakfast log
    SELECT
        FL.[Id] AS [FoodLogId],
        FN.[Id] AS [FoodNutritionId],
        2 AS [Unit]
    FROM [dbo].[FoodLogs] FL
    INNER JOIN [dbo].[Users] U ON U.[Id] = FL.[UserId]
    INNER JOIN [dbo].[FoodNutritions] FN ON FN.[Name] = 'Egg'
    WHERE U.[Email] = 'demo@nutritiontracker.local'
      AND FL.[DateTime] = CAST('2026-07-18T08:00:00' AS datetime)

    UNION ALL

    SELECT
        FL.[Id],
        FN.[Id],
        1
    FROM [dbo].[FoodLogs] FL
    INNER JOIN [dbo].[Users] U ON U.[Id] = FL.[UserId]
    INNER JOIN [dbo].[FoodNutritions] FN ON FN.[Name] = 'Blueberry'
    WHERE U.[Email] = 'demo@nutritiontracker.local'
      AND FL.[DateTime] = CAST('2026-07-18T08:00:00' AS datetime)

    UNION ALL

    -- Athlete User lunch log
    SELECT
        FL.[Id],
        FN.[Id],
        2
    FROM [dbo].[FoodLogs] FL
    INNER JOIN [dbo].[Users] U ON U.[Id] = FL.[UserId]
    INNER JOIN [dbo].[FoodNutritions] FN ON FN.[Name] = 'Salmon'
    WHERE U.[Email] = 'athlete@nutritiontracker.local'
      AND FL.[DateTime] = CAST('2026-07-18T13:00:00' AS datetime)

    UNION ALL

    SELECT
        FL.[Id],
        FN.[Id],
        1
    FROM [dbo].[FoodLogs] FL
    INNER JOIN [dbo].[Users] U ON U.[Id] = FL.[UserId]
    INNER JOIN [dbo].[FoodNutritions] FN ON FN.[Name] = 'Kale'
    WHERE U.[Email] = 'athlete@nutritiontracker.local'
      AND FL.[DateTime] = CAST('2026-07-18T13:00:00' AS datetime)

    UNION ALL

    -- Cutting User dinner log
    SELECT
        FL.[Id],
        FN.[Id],
        2
    FROM [dbo].[FoodLogs] FL
    INNER JOIN [dbo].[Users] U ON U.[Id] = FL.[UserId]
    INNER JOIN [dbo].[FoodNutritions] FN ON FN.[Name] = 'Minced Meat'
    WHERE U.[Email] = 'cutting@nutritiontracker.local'
      AND FL.[DateTime] = CAST('2026-07-18T19:00:00' AS datetime)

    UNION ALL

    SELECT
        FL.[Id],
        FN.[Id],
        1
    FROM [dbo].[FoodLogs] FL
    INNER JOIN [dbo].[Users] U ON U.[Id] = FL.[UserId]
    INNER JOIN [dbo].[FoodNutritions] FN ON FN.[Name] = 'Carrot'
    WHERE U.[Email] = 'cutting@nutritiontracker.local'
      AND FL.[DateTime] = CAST('2026-07-18T19:00:00' AS datetime)
)
MERGE [dbo].[FoodItems] AS Target
USING SourceItems AS Source
ON Target.[FoodLogId] = Source.[FoodLogId]
   AND Target.[FoodNutritionId] = Source.[FoodNutritionId]
   AND Target.[Unit] = Source.[Unit]
WHEN NOT MATCHED THEN
    INSERT ([Id], [FoodNutritionId], [Unit], [FoodLogId])
    VALUES (NEWID(), Source.[FoodNutritionId], Source.[Unit], Source.[FoodLogId]);

SELECT COUNT(*) AS [Total FoodItems after seed] FROM [dbo].[FoodItems];
GO
