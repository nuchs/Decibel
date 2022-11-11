CREATE TYPE dbo.InventoryItem AS TABLE
(
	[Name] NVARCHAR(50),
	SupplierId INT NOT NULL default 3,
	Price DECIMAL (18, 4) NULL,
	PRIMARY KEY (
		Name, Price
	),
	INDEX IX_InventoryItem_Price (
		Price
	)
)