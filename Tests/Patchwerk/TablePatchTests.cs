namespace Tests.Patchwerk;

internal class TablePatchTests : BasePatchwerkTest
{
    [Test]
    public void Add()
    {
        var expected = "CREATE TABLE blah (a int)";
        var before = MakeDb();
        var after = MakeDb(expected);

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void NoChange()
    {
        var before = MakeDb("CREATE TABLE blah (a int)");
        var after = MakeDb("CREATE TABLE blah (a int)");

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Remove()
    {
        var before = MakeDb("CREATE TABLE blah (a int)");
        var after = MakeDb();
        var expected = "DROP TABLE blah";

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }
}
