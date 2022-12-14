namespace Tests.Mast.Dbo;

public class PrimaryKeyTests : BaseMastTest
{
    [Test]
    [TestCase("", false)]
    [TestCase("NONCLUSTERED", false)]
    [TestCase("CLUSTERED", true)]
    public void Clustered(string clustered, bool expected)
    {
        var type = $"CREATE TYPE dbo.stub AS TABLE (stub int primary key {clustered})";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First().PrimaryKey;

        Assert.That(result?.Clustered, Is.EqualTo(expected));
    }

    [Test]
    public void Content()
    {
        var expected = "primary key clustered";
        var type = $"CREATE TYPE dbo.stub AS TABLE (stub int {expected})";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First().PrimaryKey;

        Assert.That(result?.Content, Is.EqualTo(expected));
    }

    [Test]
    public void NoNameOnColumn()
    {
        var type = $"CREATE TYPE dbo.stub AS TABLE (stub int primary key)";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First().PrimaryKey;

        Assert.That(result?.Name, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Name()
    {
        var expected = "Slartibartfast";
        var type = $"CREATE TABLE dbo.stub (stub int, constraint {expected} primary key (stub))";

        parser.Parse(db, type);
        var result = db.Tables.First().PrimaryKey;

        Assert.That(result?.Name, Is.EqualTo(expected));
    }

    [Test]
    public void SingleColumnCount()
    {
        var expected = "col_p";
        var type = $"CREATE TYPE dbo.stub AS TABLE ({expected} int primary key)";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First().PrimaryKey;

        Assert.That(result?.Columns.Count(), Is.EqualTo(1));
    }

    [Test]
    public void SingleColumn()
    {
        var expected = "col_p";
        var type = $"CREATE TYPE dbo.stub AS TABLE ({expected} int primary key)";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First().PrimaryKey;

        Assert.That(result?.Columns.First().Name, Is.EqualTo(expected));
    }

    [Test]
    public void CompoundCount()
    {
        var type = $"""
            CREATE TYPE dbo.stub AS TABLE
            (
                col1  int, 
                stub1 int, 
                col2  int, 
                stub2 int, 
                col3  int, 
                primary key (col1, col2, col3)
            )
            """;

        parser.Parse(db, type);
        var result = db.TableTypes.First().PrimaryKey;

        Assert.That(result?.Columns.Count(), Is.EqualTo(3));
    }

    [Test]
    public void CompoundColumns()
    {
        var type = $"""
            CREATE TYPE dbo.stub AS TABLE
            (
                col1  int, 
                col2  int, 
                primary key (col1, col2)
            )
            """;

        parser.Parse(db, type);
        var result = db.TableTypes.First().PrimaryKey;

        Assert.That(result?.Columns.Select(c => c.Name), Is.EquivalentTo(new[] { "col1", "col2" }));
    }
}
