namespace Tests.Mast;

public class ProcedureTests : BaseMastTest
{
    [Test]
    [TestCase("bear", "bear")]
    [TestCase("[bracketed]", "bracketed")]
    public void Name(string name, string expected)
    {
        var proc = $"CREATE PROCEDURE dbo.{name} AS SELECT 1";

        db.AddFromTsqlScript(proc);
        var result = db.Procedures.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("bear", "bear")]
    [TestCase("[bracketed]", "bracketed")]
    public void Schema(string schema, string expected)
    {
        var proc = $"CREATE PROCEDURE {schema}.StubName AS SELECT 1";

        db.AddFromTsqlScript(proc);
        var result = db.Procedures.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }

    [Test]
    public void Content()
    {
        var expected = """
            CREATE PROCEDURE dbo.stub
            AS
            BEGIN
                DECLARE @val int;
                set @val = 42;
                SELECT @val
            END
            """;

        db.AddFromTsqlScript(expected);
        var result = db.Procedures.First();

        Assert.That(result.Content, Is.EqualTo(expected.Trim()));
    }

    [Test]
    [TestCase("", 0)]
    [TestCase("@a int out", 1)]
    [TestCase("@a int, @b int, @c int", 3)]
    public void ParameterCount(string paramList, int expected)
    {
        var function = $"""
            CREATE PROCEDURE dbo.stub
            {paramList}
            AS
            BEGIN
                RETURN SELECT 1
            END
            """;

        db.AddFromTsqlScript(function);
        var result = db.Procedures.First();

        Assert.That(result.Parameters, Has.Exactly(expected).Items);
    }
}
