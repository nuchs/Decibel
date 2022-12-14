namespace Tests.Mast;

public class FunctionTests : BaseMastTest
{
    [Test]
    [TestCase("bear", "bear")]
    [TestCase("[bracketed]", "bracketed")]
    public void Name(string name, string expected)
    {
        var function = $"CREATE FUNCTION dbo.{name} () RETURNS TABLE AS RETURN SELECT 1";

        parser.Parse(db, function);
        var result = db.Functions.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("bear", "bear")]
    [TestCase("[bracketed]", "bracketed")]
    public void Schema(string schema, string expected)
    {
        var function = $"CREATE FUNCTION {schema}.StubName () RETURNS TABLE AS RETURN SELECT 1";

        parser.Parse(db, function);
        var result = db.Functions.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }

    [Test]
    public void Content()
    {
        var expected = """
            CREATE FUNCTION dbo.stub()
            RETURNS INT
            AS
            BEGIN
                DECLARE @val int;
                set @val = 42;
                return @val;
            END
            """;

        parser.Parse(db, expected);
        var result = db.Functions.First();

        Assert.That(result.Content, Is.EqualTo(expected.Trim()));
    }

    [Test]
    [TestCase("", 0)]
    [TestCase("@a int", 1)]
    [TestCase("@a int, @b int, @c int", 3)]
    public void ParameterCount(string paramList, int expected)
    {
        var function = $"CREATE FUNCTION dbo.stub({paramList}) RETURNS TABLE AS RETURN SELECT 1";

        parser.Parse(db, function);
        var result = db.Functions.First();

        Assert.That(result.Parameters, Has.Exactly(expected).Items);
    }

    [Test]
    [TestCase("INT", "1")]
    [TestCase("varchar(max)", "'wibble'")]
    [TestCase("nchar(10)", "'bacon'")]
    public void ReturnScalerType(string expected, string returnValue)
    {
        var function = $"""
            CREATE FUNCTION dbo.stub ()
            RETURNS {expected}
            AS
            BEGIN
            RETURN {returnValue};
            END
            """;

        parser.Parse(db, function);
        var result = db.Functions.First();

        Assert.That(result.ReturnType, Is.EqualTo(expected));
    }

    [Test]
    public void ReturnTableType()
    {
        var expected = "SELECT 1";
        var function = $"""
            CREATE FUNCTION dbo.stub ()
            RETURNS TABLE
            RETURN {expected}
            """;

        parser.Parse(db, function);
        var result = db.Functions.First();

        Assert.That(result.ReturnType, Is.EqualTo(expected));
    }

    [Test]
    public void ReturnTableTypeInMultiStatementFunction()
    {
        var expected = "@retVal TABLE ( Id int )";
        var function = $"""
            CREATE FUNCTION dbo.stub ()
            RETURNS {expected}
            AS
            BEGIN
            DECLARE @val int
            SET @val = 1
            INSERT INTO @retVal SELECT @val
            RETURN
            END
            """;

        parser.Parse(db, function);
        var result = db.Functions.First();

        Assert.That(result.ReturnType, Is.EqualTo(expected));
    }
}
