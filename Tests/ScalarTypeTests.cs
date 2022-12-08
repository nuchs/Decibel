using Mast;
using Mast.Dbo;

namespace Tests;

internal class ScalarTypeTests
{
    private Database db = new();
    private ScriptParser parser = new();

    [Test]
    public void BareName()
    {
        var expected = "bareNakedName";
        var function = $"CREATE TYPE dbo.{expected} FROM INT";

        parser.Parse(db, function);
        var result = db.ScalarTypes.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    public void BareSchema()
    {
        var expected = "nudeSchema";
        var function = $"CREATE TYPE {expected}.StubName FROM INT";

        parser.Parse(db, function);
        var result = db.ScalarTypes.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }

    [Test]
    public void BracketedName()
    {
        var expected = "Don't bracket me";
        var function = $"CREATE TYPE dbo.[{expected}] FROM INT";

        parser.Parse(db, function);
        var result = db.ScalarTypes.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    public void BracketedSchema()
    {
        var expected = "Hyphenate-this";
        var function = $"CREATE TYPE [{expected}].StubName FROM INT";

        parser.Parse(db, function);
        var result = db.ScalarTypes.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", true)]
    [TestCase(" NULL", true)]
    [TestCase(" NOT NULL", false)]
    public void Nullability(string nullSpecifier, bool nullable)
    {
        var function = $"CREATE TYPE dbo.stub FROM INT{nullSpecifier}";

        parser.Parse(db, function);
        var result = db.ScalarTypes.First();

        Assert.That(result.IsNullable, Is.EqualTo(nullable));
    }

    [Test]
    [TestCase("1")]
    [TestCase("200")]
    [TestCase("max")]
    public void ParameterisedType(string expected)
    {
        var function = $"CREATE TYPE dbo.stub FROM NVARCHAR({expected})";

        parser.Parse(db, function);
        var result = db.ScalarTypes.First();

        Assert.That(result.Parameters, Is.EquivalentTo(new List<string> { expected }));
    }

    [SetUp]
    public void Setup() => db = new();

    [Test]
    public void UnparameterisedType()
    {
        var function = $"CREATE TYPE dbo.stub FROM INT";

        parser.Parse(db, function);
        var result = db.ScalarTypes.First();

        Assert.That(result.Parameters, Is.Empty);
    }
}
