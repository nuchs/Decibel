CREATE FUNCTION dbo.IsTurkeyFast (
	@TurkeyId int not null,
	@Pointless nvarchar(10) null,
	@dflt int = 42
)
RETURNS int
AS
BEGIN
	DECLARE @IsFast int;
	SET @IsFast = 0

	DECLARE @Speed int
	SET @Speed = (SELECT TOP 1 c.Speed FROM dbo.Turkey c WHERE c.Id = @TurkeyId)

	IF(@Speed > 5)
		SET @IsFast = 1
	RETURN @IsFast
END