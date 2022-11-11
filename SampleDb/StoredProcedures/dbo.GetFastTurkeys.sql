CREATE PROCEDURE dbo.GetFastTurkeys
AS
BEGIN
	SELECT * FROM dbo.Turkey t WHERE dbo.IsTurkeyFast(t.id) = 1
END