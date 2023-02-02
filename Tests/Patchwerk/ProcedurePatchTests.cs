namespace Tests.Patchwerk;

internal class ProcedurePatchTests : BasePatchwerkTest
{
    [Test]
    public void Add()
    {
        var expected = "CREATE PROCEDURE bb.blah AS BEGIN SELECT 1 END";
        var before = MakeDb();
        var after = MakeDb(expected);

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ChangeContent()
    {
        var before = MakeDb("CREATE PROCEDURE bb.blah AS BEGIN SELECT 1 END");
        var after = MakeDb("CREATE PROCEDURE bb.blah AS BEGIN SELECT 22 END");
        var expected = """
            DROP PROCEDURE bb.blah
            GO

            CREATE PROCEDURE bb.blah AS BEGIN SELECT 22 END
            """;

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void NoChange()
    {
        var before = MakeDb("CREATE PROCEDURE bb.blah AS BEGIN SELECT 1 END");
        var after = MakeDb("CREATE PROCEDURE bb.blah AS BEGIN SELECT 1 END");

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Remove()
    {
        var before = MakeDb("CREATE PROCEDURE bb.blah AS BEGIN SELECT 1 END");
        var after = MakeDb();
        var expected = "DROP PROCEDURE bb.blah";

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }
}
