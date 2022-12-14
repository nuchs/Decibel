namespace Tests;

public class TableTests : BaseMastTest
{
    [Test]
    public void BareName()
    {
        var expected = "bareNakedName";
        var type = $"CREATE TABLE dbo.{expected} (StubColumn int)";

        parser.Parse(db, type);
        var result = db.Tables.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    public void BareSchema()
    {
        var expected = "nudeSchema";
        var type = $"CREATE TABLE {expected}.StubName (StubColumn int)";

        parser.Parse(db, type);
        var result = db.Tables.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }

    [Test]
    public void BracketedName()
    {
        var expected = "Don't bracket me";
        var type = $"CREATE TABLE dbo.[{expected}] (StubColumn int)";

        parser.Parse(db, type);
        var result = db.Tables.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    public void BracketedSchema()
    {
        var expected = "Hyphenate-this";
        var type = $"CREATE TABLE [{expected}].StubName (StubColumn int)";

        parser.Parse(db, type);
        var result = db.Tables.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }
}