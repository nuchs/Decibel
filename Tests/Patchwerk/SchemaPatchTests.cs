namespace Tests.Patchwerk;

internal class SchemaPatchTests : BasePatchwerkTest
{
    [Test]
    public void AddNew()
    {
        var before = MakeDb();
        var after = MakeDb("CREATE SCHEMA blah");
        var expected = "CREATE SCHEMA blah";

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void RemoveUnused()
    {
        var before = MakeDb("CREATE SCHEMA blah");
        var after = MakeDb();
        var expected = "DROP SCHEMA blah";

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void NoChange()
    {
        var before = MakeDb("CREATE SCHEMA blah");
        var after = MakeDb("CREATE SCHEMA blah");

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.Empty);
    }
}
