namespace Tests.Mast;

public class ScalarTypeTests : BaseMastTest
{
    [Test]
    [TestCase("bear", "bear")]
    [TestCase("[bracketed]", "bracketed")]
    public void Name(string name, string expected)
    {
        var script = $"CREATE TYPE dbo.{name} FROM INT";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.ScalarTypes.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("bear", "bear")]
    [TestCase("[bracketed]", "bracketed")]
    public void Schema(string schema, string expected)
    {
        var script = $"CREATE TYPE {schema}.StubName FROM INT";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.ScalarTypes.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }

    [Test]
    public void Content()
    {
        var script = "CREATE TYPE dbo.stub FROM int";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.ScalarTypes.First();

        Assert.That(result.Content, Is.EqualTo(script));
    }

    [Test]
    [TestCase("", true)]
    [TestCase(" NULL", true)]
    [TestCase(" NOT NULL", false)]
    public void Nullability(string nullSpecifier, bool nullable)
    {
        var script = $"CREATE TYPE dbo.stub FROM INT{nullSpecifier}";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.ScalarTypes.First();

        Assert.That(result.IsNullable, Is.EqualTo(nullable));
    }

    [Test]
    [TestCase("1")]
    [TestCase("200")]
    [TestCase("max")]
    public void ParameterisedType(string expected)
    {
        var script = $"CREATE TYPE dbo.stub FROM NVARCHAR({expected})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.ScalarTypes.First();

        Assert.That(result.Parameters, Is.EquivalentTo(new List<string> { expected }));
    }

    [Test]
    public void UnparameterisedType()
    {
        var script = $"CREATE TYPE dbo.stub FROM INT";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.ScalarTypes.First();

        Assert.That(result.Parameters, Is.Empty);
    }
}
