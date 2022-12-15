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
}
