namespace Tests;

public class CheckConstraintTests : BaseMastTest
{
    [Test]
    public void ColumnContent()
    {
        var expected = "Check(stub < 0)";
        var type = $"CREATE TYPE dbo.stub AS TABLE (stub int {expected})";

        parser.Parse(db, type);
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Check?.Content, Is.EqualTo(expected));
    }
}
