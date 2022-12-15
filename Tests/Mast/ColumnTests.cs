using Mast.Dbo;

namespace Tests.Mast;

public class ColumnTests : BaseMastTest
{
    [Test]
    [TestCase("", null)]
    [TestCase("CHECK(stub > 0)", "CHECK(stub > 0)")]
    public void CheckConstraints(string constraint, string? expected)
    {
        var script = $"CREATE TABLE dbo.stub (stub int {constraint})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First().Columns.First();

        Assert.That(result.Check?.Content, Is.EqualTo(expected));
    }

    [Test]
    public void Content()
    {
        var expected = "stub int default 2";
        var script = $"CREATE TYPE dbo.stub AS TABLE ({expected})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Content, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("int", "", "int")]
    [TestCase("nvarchar(max)", "", "nvarchar")]
    [TestCase("MySchema.MyType", "MySchema", "MyType")]
    public void DataType(string type, string schema, string name)
    {
        FullyQualifiedName expected = new(schema, name);
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub {type})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.DataType, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", null)]
    [TestCase("default 1", "default 1")]
    public void Default(string defaultConstraint, string? expected)
    {
        var script = $"CREATE TABLE dbo.stub (stub int {defaultConstraint})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First().Columns.First();

        Assert.That(result.Default?.Content, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", null)]
    [TestCase("identity", "identity")]
    public void Identity(string identityConstraint, string? expected)
    {
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub int {identityConstraint})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Identity?.Content, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("Michael", "Michael")]
    [TestCase("[Michael]", "Michael")]
    public void Name(string name, string expected)
    {
        var script = $"CREATE TYPE dbo.stub AS TABLE ({name} int)";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", true)]
    [TestCase(" NULL", true)]
    [TestCase(" NOT NULL", false)]
    public void Nullability(string nullSpec, bool? expected)
    {
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub int{nullSpec})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.IsNullable, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", null)]
    [TestCase("UNIQUE", "UNIQUE")]
    public void UniqueConstraints(string constraint, string? expected)
    {
        var script = $"CREATE TABLE dbo.stub (stub int {constraint})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First().Columns.First();

        Assert.That(result.Unique?.Content, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", null)]
    [TestCase("references dbo.stuble (col1)", "references dbo.stuble (col1)")]
    public void ForeginKey(string constraint, string? expected)
    {
        var script = $"CREATE TABLE dbo.stub (stub int {constraint})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First().Columns.First();

        Assert.That(result.ForeginKey?.Content, Is.EqualTo(expected));
    }
}
