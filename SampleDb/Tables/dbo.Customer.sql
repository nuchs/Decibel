CREATE TABLE [dbo].[Customer]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(MAX) NULL, 
    [FamilyId] INT NULL, 
    [Enroled] DATETIME2 NULL
)
