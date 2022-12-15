using Mast.Dbo;

namespace Tests.Mast;

public class UserTests : BaseMastTest
{
    [Test]
    public void Content()
    {
        var script = $"CREATE USER IgglePiggle FOR LOGIN NGIgPig WITH DEFAULT_SCHEMA = dbo";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Users.First();

        Assert.That(result.Content, Is.EqualTo(script));
    }

    [Test]
    [TestCase("", "")]
    [TestCase(", DEFAULT_LANGUAGE = NONE", "NONE")]
    [TestCase(", DEFAULT_LANGUAGE = [English]", "English")]
    public void DefaultLanguage(string sid, string expected)
    {
        var script = $"CREATE USER stub with password = '123' {sid}";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Users.First();

        Assert.That(result.DefaultLanguage, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", "")]
    [TestCase("with DEFAULT_SCHEMA = spankme", "spankme")]
    public void DefaultSchema(string schema, string expected)
    {
        var script = $"CREATE USER stub {schema}";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Users.First();

        Assert.That(result.DefaultSchema, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", "")]
    [TestCase(", sid = 0x01050000000000090300000063FF0451A9E7664BA705B10E37DDC4B7", "0x01050000000000090300000063FF0451A9E7664BA705B10E37DDC4B7")]
    public void DefaultSid(string sid, string expected)
    {
        var script = $"CREATE USER stub with password = '123' {sid}";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Users.First();

        Assert.That(result.Sid, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("bear", "bear")]
    [TestCase("[bracketed]", "bracketed")]
    public void Identifier(string name, string bareName)
    {
        FullyQualifiedName expected = new(string.Empty, bareName);
        var script = $"CREATE USER {name}";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Users.First();

        Assert.That(result.Identifier, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", "")]
    [TestCase("FOR LOGIN pob", "pob")]
    public void Login(string login, string expected)
    {
        var script = $"CREATE USER stub {login}";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Users.First();

        Assert.That(result.Login, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", "")]
    [TestCase("with password = '123456'", "123456")]
    public void Password(string password, string expected)
    {
        var script = $"CREATE USER stub {password}";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Users.First();

        Assert.That(result.Password, Is.EqualTo(expected));
    }
}
