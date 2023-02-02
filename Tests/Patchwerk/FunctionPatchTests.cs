namespace Tests.Patchwerk;

internal class FunctionPatchTests : BasePatchwerkTest
{
    [Test]
    public void Add()
    {
        var expected = "CREATE FUNCTION bb.blah() RETURNS INT AS BEGIN RETURN 1 END";
        var before = MakeDb();
        var after = MakeDb(expected);

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ChangeContent()
    {
        var before = MakeDb("CREATE FUNCTION bb.blah() RETURNS INT AS BEGIN RETURN 1 END");
        var after = MakeDb("CREATE FUNCTION bb.blah() RETURNS INT AS BEGIN RETURN 22 END");
        var expected = """
            DROP FUNCTION bb.blah
            GO

            CREATE FUNCTION bb.blah() RETURNS INT AS BEGIN RETURN 22 END
            """;

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void NoChange()
    {
        var before = MakeDb("CREATE FUNCTION bb.blah() RETURNS INT AS BEGIN RETURN 1 END");
        var after = MakeDb("CREATE FUNCTION bb.blah() RETURNS INT AS BEGIN RETURN 1 END");

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Remove()
    {
        var before = MakeDb("CREATE FUNCTION bb.blah() RETURNS INT AS BEGIN RETURN 1 END");
        var after = MakeDb();
        var expected = "DROP FUNCTION bb.blah";

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }
}
