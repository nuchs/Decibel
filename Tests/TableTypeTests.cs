namespace Tests;

public class TableTypeTests : BaseMastTest
{
    [Test]
    public void BareName()
    {
        var expected = "bareNakedName";
        var type = $"CREATE TYPE dbo.{expected} AS TABLE (StubColumn int)";

        parser.Parse(db, type);
        var result = db.TableTypes.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    public void BareSchema()
    {
        var expected = "nudeSchema";
        var type = $"CREATE TYPE {expected}.StubName AS TABLE (StubColumn int)";

        parser.Parse(db, type);
        var result = db.TableTypes.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }

    [Test]
    public void BracketedName()
    {
        var expected = "Don't bracket me";
        var type = $"CREATE TYPE dbo.[{expected}] AS TABLE (StubColumn int)";

        parser.Parse(db, type);
        var result = db.TableTypes.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    public void BracketedSchema()
    {
        var expected = "Hyphenate-this";
        var type = $"CREATE TYPE [{expected}].StubName AS TABLE (StubColumn int)";

        parser.Parse(db, type);
        var result = db.TableTypes.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }

    [Test]
    public void CompoundPrimaryKey()
    {
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
    public void NumberChecks()
    {
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
    public void PrimaryKey()
    {
    }
}