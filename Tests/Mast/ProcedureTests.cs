namespace Tests.Mast;

public class ProcedureTests : BaseMastTest
{
    [Test]
    [TestCase("bear", "bear")]
    [TestCase("[bracketed]", "bracketed")]
    public void Name(string name, string expected)
    {
        var script = $"CREATE PROCEDURE dbo.{name} AS SELECT 1";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Procedures.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("bear", "bear")]
    [TestCase("[bracketed]", "bracketed")]
    public void Schema(string schema, string expected)
    {
        var script = $"CREATE PROCEDURE {schema}.StubName AS SELECT 1";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Procedures.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }

    [Test]
    public void Content()
    {
        var script = """
            CREATE PROCEDURE dbo.stub
            AS
            BEGIN
                DECLARE @val int;
                set @val = 42;
                SELECT @val
            END
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Procedures.First();

        Assert.That(result.Content, Is.EqualTo(script.Trim()));
    }

    [Test]
    [TestCase("", 0)]
    [TestCase("@a int out", 1)]
    [TestCase("@a int, @b int, @c int", 3)]
    public void ParameterCount(string paramList, int expected)
    {
        var script = $"""
            CREATE PROCEDURE dbo.stub
            {paramList}
            AS
            BEGIN
                RETURN SELECT 1
            END
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Procedures.First();

        Assert.That(result.Parameters, Has.Exactly(expected).Items);
    }
}
