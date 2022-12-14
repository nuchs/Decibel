namespace Tests.Mast.Dbo;

public class ColumnTests : BaseMastTest
{
    [Test]
    [TestCase("", null)]
    [TestCase("CHECK(stub > 0)", "CHECK(stub > 0)")]
    public void CheckConstraints(string constraint, string? expected)
    {
        var table = $"CREATE TABLE dbo.stub (stub int {constraint})";

        parser.Parse(db, table);
        var result = db.Tables.First().Columns.First();

        Assert.That(result.Check?.Content, Is.EqualTo(expected));
    }

    [Test]
    public void Content()
    {
        var expected = "stub int default 2";
        var type = $"CREATE TYPE dbo.stub AS TABLE ({expected})";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Content, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("int")]
    [TestCase("nvarchar(max)")]
    public void DataType(string expected)
    {
        var type = $"CREATE TYPE dbo.stub AS TABLE (stub {expected})";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.DataType.ToString(), Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", null)]
    [TestCase("default 1", "default 1")]
    public void Default(string defaultConstraint, string? expected)
    {
        var table = $"CREATE TABLE dbo.stub (stub int {defaultConstraint})";

        parser.Parse(db, table);
        var result = db.Tables.First().Columns.First();

        Assert.That(result.Default?.Content, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", null)]
    [TestCase("identity", "identity")]
    public void Identity(string identityConstraint, string? expected)
    {
        var type = $"CREATE TYPE dbo.stub AS TABLE (stub int {identityConstraint})";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Identity?.Content, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("Michael", "Michael")]
    [TestCase("[Michael]", "Michael")]
    public void Name(string name, string expected)
    {
        var type = $"CREATE TYPE dbo.stub AS TABLE ({name} int)";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", true)]
    [TestCase(" NULL", true)]
    [TestCase(" NOT NULL", false)]
    public void Nullability(string nullSpec, bool? expected)
    {
        var type = $"CREATE TYPE dbo.stub AS TABLE (stub int{nullSpec})";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.IsNullable, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", null)]
    [TestCase("UNIQUE", "UNIQUE")]
    public void UniqueConstraints(string constraint, string? expected)
    {
        var table = $"CREATE TABLE dbo.stub (stub int {constraint})";

        parser.Parse(db, table);
        var result = db.Tables.First().Columns.First();

        Assert.That(result.Unique?.Content, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", null)]
    [TestCase("references dbo.stuble (col1)", "references dbo.stuble (col1)")]
    public void ForeginKey(string constraint, string? expected)
    {
        var table = $"CREATE TABLE dbo.stub (stub int {constraint})";

        parser.Parse(db, table);
        var result = db.Tables.First().Columns.First();

        Assert.That(result.ForeginKey?.Content, Is.EqualTo(expected));
    }
}
