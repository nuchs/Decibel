namespace Tests.Mast;   

public class SchemaTests : BaseMastTest
{
    [Test]
    public void Content()
    {
        var expected = $"CREATE SCHEMA splat AUTHORIZATION mrblobby";

        db.AddFromTsqlScript(expected);
        var result = db.Schemas.First();

        Assert.That(result.Content, Is.EqualTo(expected));
    }

    [Test]
    public void Name()
    {
        var expected = "splat";
        var schema = $"CREATE SCHEMA {expected} AUTHORIZATION mrblobby";

        db.AddFromTsqlScript(schema);
        var result = db.Schemas.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    public void Owner()
    {
        var expected = "mrBlobby";
        var schema = $"CREATE SCHEMA splat AUTHORIZATION {expected}";

        db.AddFromTsqlScript(schema);
        var result = db.Schemas.First();

        Assert.That(result.Owner, Is.EqualTo(expected));
    }
}
