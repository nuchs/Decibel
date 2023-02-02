namespace Tests.Patchwerk;

internal class ViewPatchTests : BasePatchwerkTest
{
    [Test]
    public void Add()
    {
        var expected = "CREATE VIEW bb.blah AS SELECT 1";
        var before = MakeDb();
        var after = MakeDb(expected);

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ChangeContent()
    {
        var before = MakeDb("CREATE VIEW bb.blah AS SELECT 1");
        var after = MakeDb("CREATE VIEW bb.blah AS SELECT 22");
        var expected = """
            DROP VIEW bb.blah
            GO

            CREATE VIEW bb.blah AS SELECT 22
            """;

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void NoChange()
    {
        var before = MakeDb("CREATE VIEW bb.blah AS SELECT 1");
        var after = MakeDb("CREATE VIEW bb.blah AS SELECT 1");

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Remove()
    {
        var before = MakeDb("CREATE VIEW bb.blah AS SELECT 1");
        var after = MakeDb();
        var expected = "DROP VIEW bb.blah";

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }
}
