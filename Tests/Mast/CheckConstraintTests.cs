namespace Tests.Mast;

public class CheckConstraintTests : BaseMastTest
{
    [Test]
    public void ColumnContent()
    {
        var expected = "Check(stub < 0)";
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub int {expected})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Check?.Content, Is.EqualTo(expected));
    }

    [Test]
    public void NoNameOnColumn()
    {
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub int Check(stub < 0))";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Check?.Name, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Name()
    {
        var expected = "chk1";
        var script = $"CREATE TABLE dbo.stub (stub int, constraint {expected} check(stub < 0))";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First().Checks.First();

        Assert.That(result?.Name, Is.EqualTo(expected));
    }
}
