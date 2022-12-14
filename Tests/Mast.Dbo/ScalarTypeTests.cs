namespace Tests.Mast.Dbo;

public class ScalarTypeTests : BaseMastTest
{
    [Test]
    [TestCase("bear", "bear")]
    [TestCase("[bracketed]", "bracketed")]
    public void Name(string name, string expected)
    {
        var type = $"CREATE TYPE dbo.{name} FROM INT";

        parser.Parse(db, type);
        var result = db.ScalarTypes.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("bear", "bear")]
    [TestCase("[bracketed]", "bracketed")]
    public void Schema(string schema, string expected)
    {
        var type = $"CREATE TYPE {schema}.StubName FROM INT";

        parser.Parse(db, type);
        var result = db.ScalarTypes.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }

    [Test]
    public void Content()
    {
        var expected = "CREATE TYPE dbo.stub FROM int";

        parser.Parse(db, expected);
        var result = db.ScalarTypes.First();

        Assert.That(result.Content, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", true)]
    [TestCase(" NULL", true)]
    [TestCase(" NOT NULL", false)]
    public void Nullability(string nullSpecifier, bool nullable)
    {
        var type = $"CREATE TYPE dbo.stub FROM INT{nullSpecifier}";

        parser.Parse(db, type);
        var result = db.ScalarTypes.First();

        Assert.That(result.IsNullable, Is.EqualTo(nullable));
    }

    [Test]
    [TestCase("1")]
    [TestCase("200")]
    [TestCase("max")]
    public void ParameterisedType(string expected)
    {
        var type = $"CREATE TYPE dbo.stub FROM NVARCHAR({expected})";

        parser.Parse(db, type);
        var result = db.ScalarTypes.First();

        Assert.That(result.Parameters, Is.EquivalentTo(new List<string> { expected }));
    }

    [Test]
    public void UnparameterisedType()
    {
        var function = $"CREATE TYPE dbo.stub FROM INT";

        parser.Parse(db, function);
        var result = db.ScalarTypes.First();

        Assert.That(result.Parameters, Is.Empty);
    }
}
