using Mast;
using Mast.Dbo;

namespace Tests;

public class TableTests
{
    private Database db = new();
    private ScriptParser parser = new();

    [Test]
    public void ParsingBareName()
    {
        var expected = "bareNakedName";
        var type = $"CREATE TABLE dbo.{expected} (StubColumn int)";

        parser.Parse(db, type);
        var result = db.Tables.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    public void ParsingBareSchema()
    {
        var expected = "nudeSchema";
        var type = $"CREATE TABLE {expected}.StubName (StubColumn int)";

        parser.Parse(db, type);
        var result = db.Tables.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }

    [Test]
    public void ParsingBracketedName()
    {
        var expected = "Don't bracket me";
        var type = $"CREATE TABLE dbo.[{expected}] (StubColumn int)";

        parser.Parse(db, type);
        var result = db.Tables.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    public void ParsingBracketedSchema()
    {
        var expected = "Hyphenate-this";
        var type = $"CREATE TABLE [{expected}].StubName (StubColumn int)";

        parser.Parse(db, type);
        var result = db.Tables.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }

    [SetUp]
    public void Setup() => db = new();
}