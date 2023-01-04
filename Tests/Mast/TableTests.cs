using Mast.Dbo;

namespace Tests.Mast;

public class TableTests : BaseMastTest
{
    [Test]
    public void CheckCount()
    {
        var script = $"CREATE TABLE dbo.stub (stub1 int, stub2 int, check (stub1 > 0), check(stub2 < 0))";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First();

        Assert.That(result.Checks.Count(), Is.EqualTo(2));
    }

    [Test]
    [TestCase("", null)]
    [TestCase(", check (stub1 > stub2)", "check (stub1 > stub2)")]
    public void Checks(string constraint, string? expected)
    {
        var script = $"CREATE TABLE dbo.stub (stub1 int, stub2 int{constraint})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First();

        Assert.That(result.Checks.FirstOrDefault()?.Content, Is.EqualTo(expected));
    }

    [Test]
    public void Content()
    {
        var script = """
            CREATE TABLE dbo.stub (
                [Name] NVARCHAR(50) Primary key,
                Number INT NOT NULL default 3
            )
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First();

        Assert.That(result.Content, Is.EqualTo(script));
    }

    [Test]
    [TestCase("", null)]
    [TestCase(", foreign key (stub) references dbo.elsewhere (col)", "foreign key (stub) references dbo.elsewhere (col)")]
    public void ForeignKeys(string constraint, string? expected)
    {
        var script = $"CREATE TABLE dbo.stub (stub int{constraint})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First();

        Assert.That(result.ForeignKeys.FirstOrDefault()?.Content, Is.EqualTo(expected));
    }

    [Test]
    public void ForeignKeysCount()
    {
        var script = """
        CREATE TABLE dbo.stub
        (
            stub1 int,
            stub2 int,
            foreign key (stub1) references dbo.elsewhere (col1),
            foreign key (stub2) references dbo.elsewhere (col2)
        )
        """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First();

        Assert.That(result.ForeignKeys.Count(), Is.EqualTo(2));
    }

    [Test]
    [TestCase("bear", "bear", "bear", "bear")]
    [TestCase("bear", "[bracketed]", "bear", "bracketed")]
    [TestCase("[bracketed]", "bear", "bracketed", "bear")]
    [TestCase("[bracketed]", "[bracketed]", "bracketed", "bracketed")]
    public void Identifier(string name, string schema, string bareName, string bareSchema)
    {
        var expected = FullyQualifiedName.FromSchemaName(bareSchema, bareName);
        var script = $"CREATE TABLE {schema}.{name} (StubColumn int)";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First();

        Assert.That(result.Identifier, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", null)]
    [TestCase(", index idx1 (stub1, stub2 desc)", "index idx1 (stub1, stub2 desc)")]
    public void Indices(string indices, string? expected)
    {
        var script = $"CREATE TABLE dbo.stub (stub1 int, stub2 int{indices})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First();

        Assert.That(result.Indices.FirstOrDefault()?.Content, Is.EqualTo(expected));
    }

    [Test]
    public void IndicesCount()
    {
        var script = $"CREATE TABLE dbo.stub (stub1 int, stub2 int, index idx1(stub1), index idx2(stub2))";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First();

        Assert.That(result.Indices.Count(), Is.EqualTo(2));
    }

    [Test]
    [TestCase("Stub int", 1)]
    [TestCase("Stub1 int, Stub2 int", 2)]
    public void NumberColumns(string columns, int expected)
    {
        var script = $"""
            CREATE TABLE dbo.stub (
                {columns}
            )
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First();

        Assert.That(result.Columns, Has.Exactly(expected).Items);
    }

    [Test]
    [TestCase("", null)]
    [TestCase(", primary key (stub)", "primary key (stub)")]
    public void PrimaryKey(string constraint, string? expected)
    {
        var script = $"CREATE TABLE dbo.stub (StubColumn int{constraint})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First();

        Assert.That(result.PrimaryKey?.Content, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", null)]
    [TestCase(", unique (stub1, stub2)", "unique (stub1, stub2)")]
    public void Unique(string constraint, string? expected)
    {
        var script = $"CREATE TABLE dbo.stub (stub1 int, stub2 int{constraint})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First();

        Assert.That(result.UniqueConstraints.FirstOrDefault()?.Content, Is.EqualTo(expected));
    }

    [Test]
    public void UniqueCount()
    {
        var script = $"CREATE TABLE dbo.stub (stub1 int, stub2 int, unique (stub1), unique (stub2))";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First();

        Assert.That(result.UniqueConstraints.Count(), Is.EqualTo(2));
    }

    [Test]
    [TestCase("dbo")]
    [TestCase("[dbo]")]
    public void ReferenceSchema(string schemaName)
    {
        var script = $"""
            CREATE SCHEMA {schemaName}
            GO

            CREATE table {schemaName}.stub (stub int)
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var table = db.Tables.First();
        var schema = db.Schemas.First();

        Assert.That(schema.ReferencedBy, Has.Member(table));
    }

    [Test]
    public void ReferenceType()
    {
        var expected = FullyQualifiedName.FromSchemaName("dbo", "MyType");
        var script = $"""
            CREATE TYPE my.typer FROM INT
            GO

            CREATE table {expected} (stub my.typer)
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var table = db.Tables.First();
        var scalar = db.ScalarTypes.First();

        Assert.That(scalar.ReferencedBy, Has.Member(table));
    }

    [Test]
    public void ReferenceTable_RowFk()
    {
        var expected = FullyQualifiedName.FromSchemaName("dbo", "MyTable");
        var script = $"""
            CREATE TABLE my.tab (col int)
            GO

            CREATE table {expected} (stub my.type references my.tab(col))
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var referee = db.Tables.First(t => t.Identifier == expected);
        var referent = db.Tables.First(t => t.Identifier != expected);

        Assert.That(referent.ReferencedBy, Has.Member(referee));
    }

    [Test]
    public void ReferenceTable_TableFk()
    {
        var expected = FullyQualifiedName.FromSchemaName("dbo", "MyTable");
        var script = $"""
            CREATE TABLE my.tab (col int)
            GO

            CREATE table {expected} (stub int, foreign key(stub) references my.tab(col))
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var referee = db.Tables.First(t => t.Identifier == expected);
        var referent = db.Tables.First(t => t.Identifier != expected);

        Assert.That(referent.ReferencedBy, Has.Member(referee));
    }
}