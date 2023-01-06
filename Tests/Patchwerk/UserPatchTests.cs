namespace Tests.Patchwerk;

internal class UserPatchTests : BasePatchwerkTest
{
    [Test]
    public void AddNewUser()
    {
        var before = MakeDb();
        var after = MakeDb("CREATE USER blah");
        var expected = "CREATE USER blah";

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void RemoveExistingUser()
    {
        var before = MakeDb("CREATE USER blah");
        var after = MakeDb();
        var expected = "DROP USER blah";

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ChangeDefaultSchema()
    {
        var before = MakeDb("CREATE USER blah WITH DEFAULT_SCHEMA = abc");
        var after = MakeDb("CREATE USER blah WITH DEFAULT_SCHEMA = def");
        var expected = "ALTER USER blah WITH DEFAULT_SCHEMA = def";

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ChangeLanguage()
    {
        var before = MakeDb("CREATE USER blah WITH DEFAULT_LANGUAGE = English");
        var after = MakeDb("CREATE USER blah WITH DEFAULT_LANGUAGE = French");
        var expected = "ALTER USER blah WITH DEFAULT_LANGUAGE = French";

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ChangeLogin()
    {
        var before = MakeDb("CREATE USER blah FOR LOGIN Jerry");
        var after = MakeDb("CREATE USER blah FOR LOGIN Tom");
        var expected = "ALTER USER blah WITH LOGIN = Tom";

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ChangeMultipleAttributes()
    {
        var before = MakeDb("""
            CREATE USER blah WITH
                DEFAULT_LANGUAGE = English,
                DEFAULT_SCHEMA = abc
            """);
        var after = MakeDb("""
            CREATE USER blah WITH
                DEFAULT_LANGUAGE = French,
                DEFAULT_SCHEMA = def
            """);
        var expected = "ALTER USER blah WITH DEFAULT_SCHEMA = def, DEFAULT_LANGUAGE = French";

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void NoChange()
    {
        var before = MakeDb("CREATE USER blah");
        var after = MakeDb("CREATE USER blah");

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.Empty);
    }
}
