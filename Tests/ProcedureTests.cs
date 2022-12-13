using Mast;
using Mast.Dbo;

namespace Tests;

public class ProcedureTests
{
    private Database db = new();
    private ScriptParser parser = new();

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

        parser.Parse(db, expected);
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

        parser.Parse(db, function);
        var result = db.Procedures.First();

        Assert.That(result.Parameters, Has.Exactly(expected).Items);
    }

    [Test]
    public void BareName()
    {
        var expected = "bareNakedName";
        var proc = $"CREATE PROCEDURE dbo.{expected} AS SELECT 1";

        parser.Parse(db, proc);
        var result = db.Procedures.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    public void BareSchema()
    {
        var expected = "nudeSchema";
        var proc = $"CREATE PROCEDURE {expected}.StubName AS SELECT 1";

        parser.Parse(db, proc);
        var result = db.Procedures.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }

    [Test]
    public void BracketedName()
    {
        var expected = "Don't bracket me";
        var proc = $"CREATE PROCEDURE dbo.[{expected}] AS SELECT 1";

        parser.Parse(db, proc);
        var result = db.Procedures.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    public void BracketedSchema()
    {
        var expected = "Hyphenate-this";
        var proc = $"CREATE PROCEDURE [{expected}].StubName AS SELECT 1";

        parser.Parse(db, proc);
        var result = db.Procedures.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }

    [SetUp]
    public void Setup() => db = new();
}
