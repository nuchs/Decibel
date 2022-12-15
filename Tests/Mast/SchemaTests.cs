using Mast.Dbo;

namespace Tests.Mast;   

public class SchemaTests : BaseMastTest
{
    [Test]
    public void Content()
    {
        var script = $"CREATE SCHEMA splat AUTHORIZATION mrblobby";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Schemas.First();

        Assert.That(result.Content, Is.EqualTo(script));
    }

    [Test]
    [TestCase("bear", "bear")]
    [TestCase("[bracketed]", "bracketed")]
    public void Identifier(string name, string bareName)
    {
        FullyQualifiedName expected = new(string.Empty, bareName);
        var script = $"CREATE SCHEMA {name} AUTHORIZATION mrblobby";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Schemas.First();

        Assert.That(result.Identifier, Is.EqualTo(expected));
    }

    [Test]
    public void Owner()
    {
        var expected = "mrBlobby";
        var script = $"CREATE SCHEMA splat AUTHORIZATION {expected}";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Schemas.First();

        Assert.That(result.Owner, Is.EqualTo(expected));
    }
}
