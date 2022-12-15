using Mast.Dbo;

namespace Tests.Mast;

public class FunctionTests : BaseMastTest
{
    [Test]
    public void Content()
    {
        var script = """
            CREATE FUNCTION dbo.stub()
            RETURNS INT
            AS
            BEGIN
                DECLARE @val int;
                set @val = 42;
                return @val;
            END
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Functions.First();

        Assert.That(result.Content, Is.EqualTo(script.Trim()));
    }

    [Test]
    [TestCase("bear", "bear", "bear", "bear")]
    [TestCase("bear", "[bracketed]", "bear", "bracketed")]
    [TestCase("[bracketed]", "bear", "bracketed", "bear")]
    [TestCase("[bracketed]", "[bracketed]", "bracketed", "bracketed")]
    public void Identifier(string name, string schema, string bareName, string bareSchema)
    {
        FullyQualifiedName expected = new(bareSchema, bareName);
        var script = $"CREATE FUNCTION {schema}.{name} () RETURNS TABLE AS RETURN SELECT 1";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Functions.First();

        Assert.That(result.Identifier, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", 0)]
    [TestCase("@a int", 1)]
    [TestCase("@a int, @b int, @c int", 3)]
    public void ParameterCount(string paramList, int expected)
    {
        var script = $"CREATE FUNCTION dbo.stub({paramList}) RETURNS TABLE AS RETURN SELECT 1";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Functions.First();

        Assert.That(result.Parameters, Has.Exactly(expected).Items);
    }

    [Test]
    [TestCase("INT", "1")]
    [TestCase("varchar(max)", "'wibble'")]
    [TestCase("nchar(10)", "'bacon'")]
    public void ReturnScalerType(string expected, string returnValue)
    {
        var script = $"""
            CREATE FUNCTION dbo.stub ()
            RETURNS {expected}
            AS
            BEGIN
            RETURN {returnValue};
            END
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Functions.First();

        Assert.That(result.ReturnType, Is.EqualTo(expected));
    }

    [Test]
    public void ReturnTableType()
    {
        var expected = "SELECT 1";
        var script = $"""
            CREATE FUNCTION dbo.stub ()
            RETURNS TABLE
            RETURN {expected}
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Functions.First();

        Assert.That(result.ReturnType, Is.EqualTo(expected));
    }

    [Test]
    public void ReturnTableTypeInMultiStatementFunction()
    {
        var expected = "@retVal TABLE ( Id int )";
        var script = $"""
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

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Functions.First();

        Assert.That(result.ReturnType, Is.EqualTo(expected));
    }
}
