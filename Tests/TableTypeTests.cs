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
        var result = db.TableTypes.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    public void ParsingBareSchema()
    {
        var expected = "nudeSchema";
        var type = $"CREATE TYPE {expected}.StubName AS TABLE (StubColumn int)";

        parser.Parse(db, type);
        var result = db.TableTypes.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }

    [Test]
    public void ParsingBracketedName()
    {
        var expected = "Don't bracket me";
        var type = $"CREATE TYPE dbo.[{expected}] AS TABLE (StubColumn int)";

        parser.Parse(db, type);
        var result = db.TableTypes.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    public void ParsingBracketedSchema()
    {
        var expected = "Hyphenate-this";
        var type = $"CREATE TYPE [{expected}].StubName AS TABLE (StubColumn int)";

        parser.Parse(db, type);
        var result = db.TableTypes.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }

    [Test]
    public void Content()
    {
        var expected = """
            CREATE TYPE dbo.stub AS TABLE (
                [Name] NVARCHAR(50) Primary key,
                Number INT NOT NULL default 3
            )
            """;

        parser.Parse(db, expected);
        var result = db.TableTypes.First();

        Assert.That(result.Content, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("Stub int", 1)]
    [TestCase("Stub1 int, Stub2 int", 2)]
    public void NumberColumns(string columns, int expected)
    {
        var type = $"""
            CREATE TYPE dbo.stub AS TABLE (
                {columns}
            )
            """;

        parser.Parse(db, type);
        var result = db.TableTypes.First();

        Assert.That(result.Columns, Has.Exactly(expected).Items);
    }

    [Test]
    public void NumberIndexes()
    {

    }

    [Test]
    public void NumberChecks()
    {

    }

    [Test]
    public void PrimaryKey()
    {

    }

    [Test]
    public void CompoundPrimaryKey()
    {

    }

    [SetUp]
    public void Setup() => db = new();
}