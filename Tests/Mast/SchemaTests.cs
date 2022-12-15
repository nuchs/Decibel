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
    public void Name()
    {
        var expected = "splat";
        var script = $"CREATE SCHEMA {expected} AUTHORIZATION mrblobby";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Schemas.First();

        Assert.That(result.Name, Is.EqualTo(expected));
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
