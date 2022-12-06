using Mast;
using Mast.Dbo;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Tests;

public class ProcedureTests
{
    private Database db = new();
    private ScriptParser parser = new();

    [Test]
    public void ParsingBareName()
    {
        var expected = "bareNakedName";
        var proc = $"CREATE PROCEDURE dbo.{expected} AS SELECT 1";

        parser.Parse(db, proc);
        var result = db.Procedures.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    public void ParsingBareSchema()
    {
        var expected = "nudeSchema";
        var proc = $"CREATE PROCEDURE {expected}.StubName AS SELECT 1";

        parser.Parse(db, proc);
        var result = db.Procedures.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }

    [Test]
    public void ParsingBracketedName()
    {
        var expected = "Don't bracket me";
        var proc = $"CREATE PROCEDURE dbo.[{expected}] AS SELECT 1";

        parser.Parse(db, proc);
        var result = db.Procedures.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    public void ParsingBracketedSchema()
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
