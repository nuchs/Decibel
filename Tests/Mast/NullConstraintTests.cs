namespace Tests.Mast;

public class NullConstraintTests : BaseMastTest
{
    [Test]
    [TestCase("NULL", true)]
    [TestCase("NOT NULL", false)]
    public void Nullability(string constraint, bool expected)
    {
        var script = $"CREATE TABLE dbo.stub (stub int {constraint})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First().Columns.First().Nullable;

        Assert.That(result?.IsNullable, Is.EqualTo(expected));
    }

    [Test]
    public void NoNameOnColumn()
    {
        var script = $"CREATE TABLE dbo.stub (stub int NOT NULL)";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First().Columns.First().Nullable;

        Assert.That(result?.Name, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Name()
    {
        var expected = "NN_stub";
        var script = $"CREATE TABLE dbo.stub (stub int CONSTRAINT {expected} NOT NULL)";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First().Columns.First().Nullable;

        Assert.That(result?.Name, Is.EqualTo(expected));
    }
}
