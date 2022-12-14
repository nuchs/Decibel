namespace Tests;

public class TableTypeTests : BaseMastTest
{
    [Test]
    [TestCase("bear", "bear")]
    [TestCase("[bracketed]", "bracketed")]
    public void Name(string name, string expected)
    {
        var type = $"CREATE TYPE dbo.{name} AS TABLE (StubColumn int)";

        parser.Parse(db, type);
        var result = db.TableTypes.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("bear", "bear")]
    [TestCase("[bracketed]", "bracketed")]
    public void Schema(string schema, string expected)
    {
        var type = $"CREATE TYPE {schema}.StubName AS TABLE (StubColumn int)";

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
}