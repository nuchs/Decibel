using Mast;
using Mast.Dbo;

namespace Tests;

public class TableTypeTests
{
    private Database db = new();
    private ScriptParser parser = new();

    [Test]
    public void ParsingBareName()
    {
        var expected = "bareNakedName";
        var type = $"CREATE TYPE dbo.{expected} AS TABLE (StubColumn int)";

        parser.Parse(db, type);
        var result = db.Types.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    public void ParsingBareSchema()
    {
        var expected = "nudeSchema";
        var type = $"CREATE TYPE {expected}.StubName AS TABLE (StubColumn int)";

        parser.Parse(db, type);
        var result = db.Types.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }

    [Test]
    public void ParsingBracketedName()
    {
        var expected = "Don't bracket me";
        var type = $"CREATE TYPE dbo.[{expected}] AS TABLE (StubColumn int)";

        parser.Parse(db, type);
        var result = db.Types.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    public void ParsingBracketedSchema()
    {
        var expected = "Hyphenate-this";
        var type = $"CREATE TYPE [{expected}].StubName AS TABLE (StubColumn int)";

        parser.Parse(db, type);
        var result = db.Types.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }

    [SetUp]
    public void Setup() => db = new();
}