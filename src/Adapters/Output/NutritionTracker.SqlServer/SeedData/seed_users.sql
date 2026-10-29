-- ============================================================
-- Seed: Users (dbo schema - src hexagonal project)
-- Run against the database after: dotnet ef database update
-- Idempotent: skips rows where Email already exists
-- ============================================================

USE [NutritionTracker];
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Users') AND name = 'RoleId')
BEGIN
    MERGE [dbo].[Users] AS Target
    USING (VALUES
        ('System Admin',  'admin@nutritiontracker.local',    'Admin123!',    2200.0, 260.0, 70.0, 150.0, '22222222-2222-2222-2222-222222222222'),
        ('Demo User',     'demo@nutritiontracker.local',     'demo123',      2200.0, 260.0, 70.0, 150.0, '11111111-1111-1111-1111-111111111111'),
        ('Athlete User',  'athlete@nutritiontracker.local',  'athlete123',   2800.0, 340.0, 85.0, 190.0, '11111111-1111-1111-1111-111111111111'),
        ('Cutting User',  'cutting@nutritiontracker.local',  'cutting123',   1800.0, 160.0, 55.0, 170.0, '11111111-1111-1111-1111-111111111111')
    ) AS Source ([Name], [Email], [Password], [SuggestedCalories], [SuggestedCarbs], [SuggestedFat], [SuggestedProtein], [RoleId])
    ON Target.[Email] = Source.[Email]
    WHEN NOT MATCHED THEN
        INSERT ([Id], [Name], [Email], [Password], [SuggestedCalories], [SuggestedCarbs], [SuggestedFat], [SuggestedProtein], [RoleId])
        VALUES (NEWID(), Source.[Name], Source.[Email], Source.[Password], Source.[SuggestedCalories], Source.[SuggestedCarbs], Source.[SuggestedFat], Source.[SuggestedProtein], CAST(Source.[RoleId] AS uniqueidentifier));

    UPDATE u
    SET u.[RoleId] = '22222222-2222-2222-2222-222222222222'
    FROM [dbo].[Users] u
    WHERE u.[Email] = 'admin@nutritiontracker.local';
END
ELSE
BEGIN
    MERGE [dbo].[Users] AS Target
    USING (VALUES
        ('System Admin',  'admin@nutritiontracker.local',    'Admin123!',    2200.0, 260.0, 70.0, 150.0),
        ('Demo User',     'demo@nutritiontracker.local',     'demo123',      2200.0, 260.0, 70.0, 150.0),
        ('Athlete User',  'athlete@nutritiontracker.local',  'athlete123',   2800.0, 340.0, 85.0, 190.0),
        ('Cutting User',  'cutting@nutritiontracker.local',  'cutting123',   1800.0, 160.0, 55.0, 170.0)
    ) AS Source ([Name], [Email], [Password], [SuggestedCalories], [SuggestedCarbs], [SuggestedFat], [SuggestedProtein])
    ON Target.[Email] = Source.[Email]
    WHEN NOT MATCHED THEN
        INSERT ([Id], [Name], [Email], [Password], [SuggestedCalories], [SuggestedCarbs], [SuggestedFat], [SuggestedProtein])
        VALUES (NEWID(), Source.[Name], Source.[Email], Source.[Password], Source.[SuggestedCalories], Source.[SuggestedCarbs], Source.[SuggestedFat], Source.[SuggestedProtein]);
END

SELECT COUNT(*) AS [Total Users after seed] FROM [dbo].[Users];
GO
