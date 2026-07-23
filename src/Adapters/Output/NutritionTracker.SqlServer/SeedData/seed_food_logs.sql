-- ============================================================
-- Seed: FoodLogs (dbo schema - src hexagonal project)
-- Prerequisite: seed_users.sql
-- Run against the database after: dotnet ef database update
-- Idempotent: skips rows where UserId + DateTime already exists
-- ============================================================

USE [NutritionTracker];
GO

WITH SourceLogs AS (
    SELECT
        U.[Id] AS [UserId],
        CAST('2026-07-18T08:00:00' AS datetime) AS [DateTime],
        CAST('2026-07-18T08:05:00' AS datetime) AS [CreateTime],
        CAST('2026-07-18T08:05:00' AS datetime) AS [UpdateTime],
        450.0 AS [TotalCalories],
        39.0 AS [TotalCarbs],
        24.0 AS [TotalProtein],
        26.0 AS [TotalFat]
    FROM [dbo].[Users] U
    WHERE U.[Email] = 'demo@nutritiontracker.local'

    UNION ALL

    SELECT
        U.[Id],
        CAST('2026-07-18T13:00:00' AS datetime),
        CAST('2026-07-18T13:10:00' AS datetime),
        CAST('2026-07-18T13:10:00' AS datetime),
        615.0,
        28.0,
        42.0,
        33.0
    FROM [dbo].[Users] U
    WHERE U.[Email] = 'athlete@nutritiontracker.local'

    UNION ALL

    SELECT
        U.[Id],
        CAST('2026-07-18T19:00:00' AS datetime),
        CAST('2026-07-18T19:02:00' AS datetime),
        CAST('2026-07-18T19:02:00' AS datetime),
        390.0,
        31.0,
        34.0,
        14.0
    FROM [dbo].[Users] U
    WHERE U.[Email] = 'cutting@nutritiontracker.local'
)
MERGE [dbo].[FoodLogs] AS Target
USING SourceLogs AS Source
ON Target.[UserId] = Source.[UserId]
   AND Target.[DateTime] = Source.[DateTime]
WHEN NOT MATCHED THEN
    INSERT ([Id], [DateTime], [CreateTime], [UpdateTime], [UserId], [TotalCalories], [TotalCarbs], [TotalProtein], [TotalFat])
    VALUES (NEWID(), Source.[DateTime], Source.[CreateTime], Source.[UpdateTime], Source.[UserId], Source.[TotalCalories], Source.[TotalCarbs], Source.[TotalProtein], Source.[TotalFat]);

SELECT COUNT(*) AS [Total FoodLogs after seed] FROM [dbo].[FoodLogs];
GO
