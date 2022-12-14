namespace Tests.Mast;

public class TableTests : BaseMastTest
{
    [Test]
    [TestCase("bear", "bear")]
    [TestCase("[bracketed]", "bracketed")]
    public void Name(string name, string expected)
    {
        var table = $"CREATE TABLE dbo.{name} (StubColumn int)";

        parser.Parse(db, table);
        var result = db.Tables.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("bear", "bear")]
    [TestCase("[bracketed]", "bracketed")]
    public void Schema(string schema, string expected)
    {
        var table = $"CREATE TABLE {schema}.StubName (StubColumn int)";

        parser.Parse(db, table);
        var result = db.Tables.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }

    [Test]
    public void Content()
    {
        var expected = """
            CREATE TABLE dbo.stub (
                [Name] NVARCHAR(50) Primary key,
                Number INT NOT NULL default 3
            )
            """;

        parser.Parse(db, expected);
        var result = db.Tables.First();

        Assert.That(result.Content, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("Stub int", 1)]
    [TestCase("Stub1 int, Stub2 int", 2)]
    public void NumberColumns(string columns, int expected)
    {
        var type = $"""
            CREATE TABLE dbo.stub (
                {columns}
            )
            """;

        parser.Parse(db, type);
        var result = db.Tables.First();

        Assert.That(result.Columns, Has.Exactly(expected).Items);
    }

    [Test]
    [TestCase("", null)]
    [TestCase(", primary key (stub)", "primary key (stub)")]
    public void PrimaryKey(string constraint, string? expected)
    {
        var type = $"CREATE TABLE dbo.stub (StubColumn int{constraint})";

        parser.Parse(db, type);
        var result = db.Tables.First();

        Assert.That(result.PrimaryKey?.Content, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", null)]
    [TestCase(", unique (stub1, stub2)", "unique (stub1, stub2)")]
    public void Unique(string constraint, string? expected)
    {
        var type = $"CREATE TABLE dbo.stub (stub1 int, stub2 int{constraint})";

        parser.Parse(db, type);
        var result = db.Tables.First();

        Assert.That(result.UniqueConstraints.FirstOrDefault()?.Content, Is.EqualTo(expected));
    }

    [Test]
    public void UniqueCount()
    {
        var type = $"CREATE TABLE dbo.stub (stub1 int, stub2 int, unique (stub1), unique (stub2))";

        parser.Parse(db, type);
        var result = db.Tables.First();

        Assert.That(result.UniqueConstraints.Count(), Is.EqualTo(2));
    }

    [Test]
    [TestCase("", null)]
    [TestCase(", check (stub1 > stub2)", "check (stub1 > stub2)")]
    public void Checks(string constraint, string? expected)
    {
        var type = $"CREATE TABLE dbo.stub (stub1 int, stub2 int{constraint})";

        parser.Parse(db, type);
        var result = db.Tables.First();

        Assert.That(result.Checks.FirstOrDefault()?.Content, Is.EqualTo(expected));
    }

    [Test]
    public void CheckCount()
    {
        var type = $"CREATE TABLE dbo.stub (stub1 int, stub2 int, check (stub1 > 0), check(stub2 < 0))";

        parser.Parse(db, type);
        var result = db.Tables.First();

        Assert.That(result.Checks.Count(), Is.EqualTo(2));
    }

    [Test]
    [TestCase("", null)]
    [TestCase(", index idx1 (stub1, stub2 desc)", "index idx1 (stub1, stub2 desc)")]
    public void Indices(string indices, string? expected)
    {
        var type = $"CREATE TABLE dbo.stub (stub1 int, stub2 int{indices})";

        parser.Parse(db, type);
        var result = db.Tables.First();

        Assert.That(result.Indices.FirstOrDefault()?.Content, Is.EqualTo(expected));
    }

    [Test]
    public void IndicesCount()
    {
        var type = $"CREATE TABLE dbo.stub (stub1 int, stub2 int, index idx1(stub1), index idx2(stub2))";

        parser.Parse(db, type);
        var result = db.Tables.First();

        Assert.That(result.Indices.Count(), Is.EqualTo(2));
    }

    [Test]
    [TestCase("", null)]
    [TestCase(", foreign key (stub) references dbo.elsewhere (col)", "foreign key (stub) references dbo.elsewhere (col)")]
    public void ForeignKeys(string constraint, string? expected)
    {
        var type = $"CREATE TABLE dbo.stub (stub int{constraint})";

        parser.Parse(db, type);
        var result = db.Tables.First();

        Assert.That(result.ForeignKeys.FirstOrDefault()?.Content, Is.EqualTo(expected));
    }

    [Test]
    public void ForeignKeysCount()
    {
        var type = """
        CREATE TABLE dbo.stub 
        (
            stub1 int, 
            stub2 int, 
            foreign key (stub1) references dbo.elsewhere (col1),
            foreign key (stub2) references dbo.elsewhere (col2)
        )
        """;

        parser.Parse(db, type);
        var result = db.Tables.First();

        Assert.That(result.ForeignKeys.Count(), Is.EqualTo(2));
    }
}