using Mast.Dbo;

namespace Tests.Mast;

public class TableTypeTests : BaseMastTest
{
    [Test]
    public void CheckCount()
    {
        var script = $"CREATE TYPE dbo.Stub AS TABLE (stub1 int, stub2 int, check (stub1 > 0), check(stub2 < 0))";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First();

        Assert.That(result.Checks.Count(), Is.EqualTo(2));
    }

    [Test]
    [TestCase("", null)]
    [TestCase(", check (stub1 > stub2)", "check (stub1 > stub2)")]
    public void Checks(string constraint, string? expected)
    {
        var script = $"CREATE TYPE dbo.Stub AS TABLE (stub1 int, stub2 int{constraint})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First();

        Assert.That(result.Checks.FirstOrDefault()?.Content, Is.EqualTo(expected));
    }

    [Test]
    public void Content()
    {
        var script = """
            CREATE TYPE dbo.stub AS TABLE (
                [Name] NVARCHAR(50) Primary key,
                Number INT NOT NULL default 3
            )
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First();

        Assert.That(result.Content, Is.EqualTo(script));
    }

    [Test]
    [TestCase("bear", "bear", "bear", "bear")]
    [TestCase("bear", "[bracketed]", "bear", "bracketed")]
    [TestCase("[bracketed]", "bear", "bracketed", "bear")]
    [TestCase("[bracketed]", "[bracketed]", "bracketed", "bracketed")]
    public void Identifier(string name, string schema, string bareName, string bareSchema)
    {
        var expected = FullyQualifiedName.FromSchemaName(bareSchema, bareName);
        var script = $"CREATE TYPE {schema}.{name} AS TABLE (StubColumn int)";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First();

        Assert.That(result.Identifier, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", null)]
    [TestCase(", index idx1 (stub1, stub2 desc)", "index idx1 (stub1, stub2 desc)")]
    public void Indices(string indices, string? expected)
    {
        var script = $"CREATE TYPE dbo.Stub AS TABLE (stub1 int, stub2 int{indices})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First();

        Assert.That(result.Indices.FirstOrDefault()?.Content, Is.EqualTo(expected));
    }

    [Test]
    public void IndicesCount()
    {
        var script = $"CREATE TYPE dbo.Stub AS TABLE (stub1 int, stub2 int, index idx1(stub1), index idx2(stub2))";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First();

        Assert.That(result.Indices.Count(), Is.EqualTo(2));
    }

    [Test]
    [TestCase("Stub int", 1)]
    [TestCase("Stub1 int, Stub2 int", 2)]
    public void NumberColumns(string columns, int expected)
    {
        var script = $"""
            CREATE TYPE dbo.stub AS TABLE (
                {columns}
            )
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First();

        Assert.That(result.Columns, Has.Exactly(expected).Items);
    }

    [Test]
    [TestCase("", null)]
    [TestCase(", primary key (stub)", "primary key (stub)")]
    public void PrimaryKey(string constraint, string? expected)
    {
        var script = $"CREATE TYPE dbo.Stub AS TABLE (StubColumn int{constraint})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First();

        Assert.That(result.PrimaryKey?.Content, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", null)]
    [TestCase(", unique (stub1, stub2)", "unique (stub1, stub2)")]
    public void Unique(string constraint, string? expected)
    {
        var script = $"CREATE TYPE dbo.Stub AS TABLE (stub1 int, stub2 int{constraint})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First();

        Assert.That(result.UniqueConstraints.FirstOrDefault()?.Content, Is.EqualTo(expected));
    }

    [Test]
    public void UniqueCount()
    {
        var script = $"CREATE TYPE dbo.Stub AS TABLE (stub1 int, stub2 int, unique (stub1), unique (stub2))";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First();

        Assert.That(result.UniqueConstraints.Count(), Is.EqualTo(2));
    }
}