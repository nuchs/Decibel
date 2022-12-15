namespace Tests.Mast;

public class UniqueConstraintTests : BaseMastTest
{
    [Test]
    [TestCase("", false)]
    [TestCase("NONCLUSTERED", false)]
    [TestCase("CLUSTERED", true)]
    public void Clustered(string clustered, bool expected)
    {
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub int unique {clustered})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Unique?.Clustered, Is.EqualTo(expected));
    }

    [Test]
    public void Content()
    {
        var expected = "unique clustered";
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub int {expected})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Unique?.Content, Is.EqualTo(expected));
    }

    [Test]
    public void NoNameOnColumn()
    {
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub int unique)";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Unique?.Name, Is.EqualTo(string.Empty));
    }

    [Test]
    public void SingleColumn()
    {
        var expected = "col_p";
        var script = $"CREATE TYPE dbo.stub AS TABLE ({expected} int unique)";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Unique?.Columns.Count(), Is.EqualTo(1));
        Assert.That(result.Unique?.Columns.First().Name, Is.EqualTo(expected));
    }

    [Test]
    public void Name()
    {
        var expected = "uq1";
        var script = $"CREATE TABLE dbo.stub (stub int, constraint {expected} unique (stub))";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First().UniqueConstraints.First(); ;

        Assert.That(result?.Name, Is.EqualTo(expected));
    }
}
