-- Insert Default User for Development/Testing
-- This script inserts a default user to resolve foreign key constraints
-- This user will be used temporarily until proper authentication is implemented

-- Use the Nutrition schema
USE [NutritionTracker]; -- Replace with your actual database name
GO

-- Insert default user if it doesn't exist
-- Using MERGE statement to avoid duplicates based on Email

MERGE [Nutrition].[Users] AS Target
USING (VALUES
    ('00000000-0000-0000-0000-000000000001', 'Default User', 'default@nutrition.app', 'temp_password_hash')
) AS Source ([Id], [Name], [Email], [Password])
ON Target.[Email] = Source.[Email]
WHEN NOT MATCHED THEN
    INSERT ([Id], [Name], [Email], [Password])
    VALUES (Source.[Id], Source.[Name], Source.[Email], Source.[Password]);

-- Display the results
SELECT COUNT(*) as 'Total Users in Database' FROM [Nutrition].[Users];

-- Display all users to verify
SELECT 
    [Id],
    [Name],
    [Email],
    [Password]
FROM [Nutrition].[Users]
ORDER BY [Name];

PRINT 'Default user has been successfully inserted/updated in the Users table.';
PRINT 'IMPORTANT: This is a temporary solution for development. Implement proper authentication before production!';
GO
