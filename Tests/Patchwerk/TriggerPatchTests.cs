namespace Tests.Patchwerk;

internal class TriggerPatchTests : BasePatchwerkTest
{
    [Test]
    public void Add()
    {
        var before = MakeDb();
        var after = MakeDb("CREATE TRIGGER bb.blah ON bb.mytable AFTER INSERT AS SELECT 1");
        var expected = "CREATE TRIGGER bb.blah ON bb.mytable AFTER INSERT AS SELECT 1";

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ChangeContent()
    {
        var before = MakeDb("CREATE TRIGGER bb.blah ON bb.mytable AFTER INSERT AS SELECT 1");
        var after = MakeDb("CREATE TRIGGER bb.blah ON bb.mytable INSTEAD OF INSERT AS SELECT 1");
        var expected = """
            DROP TRIGGER bb.blah
            GO

            CREATE TRIGGER bb.blah ON bb.mytable INSTEAD OF INSERT AS SELECT 1
            """;

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void NoChange()
    {
        var before = MakeDb("CREATE TRIGGER bb.blah ON bb.mytable AFTER INSERT AS SELECT 1");
        var after = MakeDb("CREATE TRIGGER bb.blah ON bb.mytable AFTER INSERT AS SELECT 1");

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Remove()
    {
        var before = MakeDb("CREATE TRIGGER bb.blah ON bb.mytable AFTER INSERT AS SELECT 1");
        var after = MakeDb();
        var expected = "DROP TRIGGER bb.blah";

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }
}
