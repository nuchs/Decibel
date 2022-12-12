using Mast;
using Mast.Dbo;
using System.Net.Http.Headers;

namespace Tests;

internal class ColumnTests
{
    private Database db = new();
    private ScriptParser parser = new();

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
    public void DefaultNameOnTableType()
    {
        var type = $"CREATE TYPE dbo.stub AS TABLE (stub int default 1)";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.DefaultName, Is.Null);
    }

    [Test]
    [TestCase("", false)]
    [TestCase("PRIMARY KEY", true)]
    public void PrimaryKey(string constraint, bool expected)
    {
        var table = $"CREATE TABLE dbo.stub (stub int {constraint})";

        parser.Parse(db, table);
        var result = db.Tables.First().Columns.First();

        Assert.That(result.IsPrimary, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", false)]
    [TestCase("PRIMARY KEY", true)]
    [TestCase("UNIQUE", true)]
    public void UniqueConstraints(string constraint, bool expected)
    {
        var table = $"CREATE TABLE dbo.stub (stub int {constraint})";

        parser.Parse(db, table);
        var result = db.Tables.First().Columns.First();

        Assert.That(result.IsUnique, Is.EqualTo(expected));
    }

    [Test]
    public void DefaultNotPresent()
    {
        var table = $"CREATE TABLE dbo.stub (stub int)";

        parser.Parse(db, table);
        var result = db.Tables.First().Columns.First();

        Assert.That(result.Default, Is.Null);
        Assert.That(result.DefaultName, Is.Null);
    }

    [Test]
    public void DefaultValue()
    {
        var expected = "1";
        var type = $"CREATE TYPE dbo.stub AS TABLE (stub int default {expected})";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Default, Is.EqualTo(expected));
    }

    [Test]
    public void Name()
    {
        var expected = "Michael";
        var type = $"CREATE TYPE dbo.stub AS TABLE ({expected} int)";

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
    public void IdentityNotPresent()
    {
        var type = $"CREATE TYPE dbo.stub AS TABLE (stub int)";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Identity, Is.Null);
    }

    [Test]
    public void BareIdentitySeed()
    {
        var type = $"CREATE TYPE dbo.stub AS TABLE (stub int identity)";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Identity?.Seed, Is.EqualTo(1));
    }

    [Test]
    public void BareIdentityIncrement()
    {
        var type = $"CREATE TYPE dbo.stub AS TABLE (stub int identity)";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Identity?.Increment, Is.EqualTo(1));
    }

    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(1)]
    public void IdentitySeed(int expected)
    {
        var type = $"CREATE TYPE dbo.stub AS TABLE (stub int identity({expected}, 1))";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Identity?.Seed, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(1)]
    [TestCase(2)]
    public void IdentityIncrement(int expected)
    {
        var type = $"CREATE TYPE dbo.stub AS TABLE (stub int identity(0, {expected}))";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Identity?.Increment, Is.EqualTo(expected));
    }

    [SetUp]
    public void Setup() => db = new();
}
