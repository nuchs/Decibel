namespace Tests.Mast;

public class PrimaryKeyTests : BaseMastTest
{
    [Test]
    [TestCase("", false)]
    [TestCase("NONCLUSTERED", false)]
    [TestCase("CLUSTERED", true)]
    public void Clustered(string clustered, bool expected)
    {
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub int primary key {clustered})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First().PrimaryKey;

        Assert.That(result?.Clustered, Is.EqualTo(expected));
    }

    [Test]
    public void Content()
    {
        var expected = "primary key clustered";
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub int {expected})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First().PrimaryKey;

        Assert.That(result?.Content, Is.EqualTo(expected));
    }

    [Test]
    public void NoNameOnColumn()
    {
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub int primary key)";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First().PrimaryKey;

        Assert.That(result?.Name, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Name()
    {
        var expected = "Slartibartfast";
        var script = $"CREATE TABLE dbo.stub (stub int, constraint {expected} primary key (stub))";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First().PrimaryKey;

        Assert.That(result?.Name, Is.EqualTo(expected));
    }

    [Test]
    public void SingleColumnCount()
    {
        var expected = "col_p";
        var script = $"CREATE TYPE dbo.stub AS TABLE ({expected} int primary key)";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First().PrimaryKey;

        Assert.That(result?.Columns.Count(), Is.EqualTo(1));
    }

    [Test]
    public void SingleColumn()
    {
        var expected = "col_p";
        var script = $"CREATE TYPE dbo.stub AS TABLE ({expected} int primary key)";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First().PrimaryKey;

        Assert.That(result?.Columns.First().Name, Is.EqualTo(expected));
    }

    [Test]
    public void CompoundCount()
    {
        var script = $"""
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

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().PrimaryKey;

        Assert.That(result?.Columns.Count(), Is.EqualTo(3));
    }

    [Test]
    public void CompoundColumns()
    {
        var script = $"""
            CREATE TYPE dbo.stub AS TABLE
            (
                col1  int, 
                col2  int, 
                primary key (col1, col2)
            )
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().PrimaryKey;

        Assert.That(result?.Columns.Select(c => c.Name), Is.EquivalentTo(new[] { "col1", "col2" }));
    }
}
