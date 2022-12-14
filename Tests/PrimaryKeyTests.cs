namespace Tests;

internal class PrimaryKeyTests : BaseMastTest
{
    [Test]
    [TestCase("", false)]
    [TestCase("NONCLUSTERED", false)]
    [TestCase("CLUSTERED", true)]
    public void Clustered(string clustered, bool expected)
    {
        var type = $"CREATE TYPE dbo.stub AS TABLE (stub int primary key {clustered})";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.PrimaryKey?.Clustered, Is.EqualTo(expected));
    }

    [Test]
    public void Content()
    {
        var expected = "primary key clustered";
        var type = $"CREATE TYPE dbo.stub AS TABLE (stub int {expected})";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.PrimaryKey?.Content, Is.EqualTo(expected));
    }

    [Test]
    public void MultiColumn()
    {
    }

    [Test]
    public void NoNameOnColumn()
    {
        var type = $"CREATE TYPE dbo.stub AS TABLE (stub int primary key)";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.PrimaryKey?.Name, Is.EqualTo(string.Empty));
    }

    [Test]
    public void SingleColumn()
    {
        var expected = "col_p";
        var type = $"CREATE TYPE dbo.stub AS TABLE ({expected} int primary key)";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.PrimaryKey?.Columns.Count(), Is.EqualTo(1));
        Assert.That(result.PrimaryKey?.Columns.First().Name, Is.EqualTo(expected));
    }
}
