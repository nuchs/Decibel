CREATE TYPE dbo.InventoryItem AS TABLE
(
	[Name] NVARCHAR(50) Primary key,
	SupplierId INT NOT NULL default 3,
	Price DECIMAL (18, 4) NULL,
	INDEX IX_InventoryItem_Price (
		Price
	)
)