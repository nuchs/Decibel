CREATE TABLE [dbo].[Customer]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [Name] NVARCHAR(MAX) NULL, 
    [Enroled] DATETIME2 NULL,
)
