CREATE PROCEDURE dbo.GetCustomers
	@CustomerList dbo.CustomerList READONLY
AS
BEGIN
	SELECT *
	FROM dbo.Customer c
	JOIN @CustomerList l ON c.Name = l.Name
END