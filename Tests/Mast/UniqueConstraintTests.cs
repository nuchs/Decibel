namespace Tests.Mast;

public class UniqueConstraintTests : BaseMastTest
{
    [Test]
    [TestCase("", false)]
    [TestCase("NONCLUSTERED", false)]
    [TestCase("CLUSTERED", true)]
    public void Clustered(string clustered, bool expected)
    {
        var type = $"CREATE TYPE dbo.stub AS TABLE (stub int unique {clustered})";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Unique?.Clustered, Is.EqualTo(expected));
    }

    [Test]
    public void Content()
    {
        var expected = "unique clustered";
        var type = $"CREATE TYPE dbo.stub AS TABLE (stub int {expected})";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Unique?.Content, Is.EqualTo(expected));
    }

    [Test]
    public void NoNameOnColumn()
    {
        var type = $"CREATE TYPE dbo.stub AS TABLE (stub int unique)";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Unique?.Name, Is.EqualTo(string.Empty));
    }

    [Test]
    public void SingleColumn()
    {
        var expected = "col_p";
        var type = $"CREATE TYPE dbo.stub AS TABLE ({expected} int unique)";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Unique?.Columns.Count(), Is.EqualTo(1));
        Assert.That(result.Unique?.Columns.First().Name, Is.EqualTo(expected));
    }
}
