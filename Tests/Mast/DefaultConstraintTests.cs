namespace Tests.Mast;

public class DefaultConstraintTests : BaseMastTest
{
    [Test]
    public void Content()
    {
        var expected = "default 1";
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub int {expected})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Default?.Content, Is.EqualTo(expected));
    }

    [Test]
    public void NameOnTableColumn()
    {
        var expected = "dft_stub";
        var script = $"CREATE TABLE dbo.stub (stub int constraint {expected} default 1)";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First().Columns.First();

        Assert.That(result.Default?.Name, Is.EqualTo(expected));
    }

    [Test]
    public void NameOnTableTypeColumn()
    {
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub int default 1)";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Default?.Name, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Value()
    {
        var expected = "1";
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub int default {expected})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Default?.Value, Is.EqualTo(expected));
    }
}
