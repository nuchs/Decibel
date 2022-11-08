CREATE FUNCTION dbo.IsCustomerFast (@CustomerId int)
RETURNS int
AS
BEGIN
	DECLARE @IsFast int;
	SET @IsFast = 0

	DECLARE @Speed int
	SET @Speed = (SELECT TOP 1 c.Speed FROM dbo.Customer c WHERE c.Id = @CustomerId)

	IF(@Speed > 5)
		SET @IsFast = 1
	RETURN @IsFast
END