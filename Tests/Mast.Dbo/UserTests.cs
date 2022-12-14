namespace Tests.Mast.Dbo;

public class UserTests : BaseMastTest
{
    [Test]
    public void Content()
    {
        var expected = $"CREATE USER IgglePiggle FOR LOGIN NGIgPig WITH DEFAULT_SCHEMA = dbo";

        parser.Parse(db, expected);
        var result = db.Users.First();

        Assert.That(result.Content, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", "")]
    [TestCase(", DEFAULT_LANGUAGE = NONE", "NONE")]
    [TestCase(", DEFAULT_LANGUAGE = [English]", "English")]
    public void DefaultLanguage(string sid, string expected)
    {
        var user = $"CREATE USER stub with password = '123' {sid}";

        parser.Parse(db, user);
        var result = db.Users.First();

        Assert.That(result.DefaultLanguage, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", "")]
    [TestCase("with DEFAULT_SCHEMA = spankme", "spankme")]
    public void DefaultSchema(string schema, string expected)
    {
        var user = $"CREATE USER stub {schema}";

        parser.Parse(db, user);
        var result = db.Users.First();

        Assert.That(result.DefaultSchema, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", "")]
    [TestCase(", sid = 0x01050000000000090300000063FF0451A9E7664BA705B10E37DDC4B7", "0x01050000000000090300000063FF0451A9E7664BA705B10E37DDC4B7")]
    public void DefaultSid(string sid, string expected)
    {
        var user = $"CREATE USER stub with password = '123' {sid}";

        parser.Parse(db, user);
        var result = db.Users.First();

        Assert.That(result.Sid, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", "")]
    [TestCase("FOR LOGIN pob", "pob")]
    public void Login(string login, string expected)
    {
        var user = $"CREATE USER stub {login}";

        parser.Parse(db, user);
        var result = db.Users.First();

        Assert.That(result.Login, Is.EqualTo(expected));
    }

    [Test]
    public void Name()
    {
        var expected = "IgglePiggle";
        var user = $"CREATE USER {expected}";

        parser.Parse(db, user);
        var result = db.Users.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", "")]
    [TestCase("with password = '123456'", "123456")]
    public void Password(string password, string expected)
    {
        var user = $"CREATE USER stub {password}";

        parser.Parse(db, user);
        var result = db.Users.First();

        Assert.That(result.Password, Is.EqualTo(expected));
    }
}
