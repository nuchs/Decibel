CREATE TABLE [dbo].[Customer]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [Name] NVARCHAR(MAX) NULL, 
    [Enroled] DATETIME2 NULL,
    d1 int default 23,
    d2 int constraint DF_d2 default 50,
    CONSTRAINT chk_d1_big check (d1 > 10)
    
)
