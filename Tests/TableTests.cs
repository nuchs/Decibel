namespace Tests;

public class TableTests : BaseMastTest
{
    [Test]
    [TestCase("bear", "bear")]
    [TestCase("[bracketed]", "bracketed")]
    public void Name(string name, string expected)
    {
        var table = $"CREATE TABLE dbo.{name} (StubColumn int)";

        parser.Parse(db, table);
        var result = db.Tables.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("bear", "bear")]
    [TestCase("[bracketed]", "bracketed")]
    public void Schema(string schema, string expected)
    {
        var table = $"CREATE TABLE {schema}.StubName (StubColumn int)";

        parser.Parse(db, table);
        var result = db.Tables.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }
}