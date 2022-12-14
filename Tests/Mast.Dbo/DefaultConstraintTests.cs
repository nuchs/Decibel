namespace Tests.Mast.Dbo;

public class DefaultConstraintTests : BaseMastTest
{
    [Test]
    public void Content()
    {
        var expected = "default 1";
        var type = $"CREATE TYPE dbo.stub AS TABLE (stub int {expected})";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Default?.Content, Is.EqualTo(expected));
    }

    [Test]
    public void NameOnTableColumn()
    {
        var expected = "dft_stub";
        var type = $"CREATE TABLE dbo.stub (stub int constraint {expected} default 1)";

        parser.Parse(db, type);
        var result = db.Tables.First().Columns.First();

        Assert.That(result.Default?.Name, Is.EqualTo(expected));
    }

    [Test]
    public void NameOnTableTypeColumn()
    {
        var type = $"CREATE TYPE dbo.stub AS TABLE (stub int default 1)";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Default?.Name, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Value()
    {
        var expected = "1";
        var type = $"CREATE TYPE dbo.stub AS TABLE (stub int default {expected})";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Default?.Value, Is.EqualTo(expected));
    }
}
