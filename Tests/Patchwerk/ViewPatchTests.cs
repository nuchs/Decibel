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
        var expected = "ALTER VIEW bb.blah AS SELECT 22";

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

    [Test]
    public void CorrectPatchOrder()
    {
        var before = MakeDb(
            "CREATE VIEW a (a1) AS SELECT 1",
            "CREATE VIEW b (b1) AS SELECT a1 FROM a",
            "CREATE VIEW c (c1) AS SELECT b1 FROM b");
        var after = MakeDb(
            "CREATE VIEW c (c2) AS SELECT b2 FROM b",
            "CREATE VIEW b (b2) AS SELECT a2 FROM a",
            "CREATE VIEW d (d1) AS SELECT 1",
            "CREATE VIEW a (a2) AS SELECT d1 FROM d");
        var expected = """
            CREATE VIEW d (d1) AS SELECT 1
            GO

            ALTER VIEW a (a2) AS SELECT d1 FROM d
            GO

            ALTER VIEW b (b2) AS SELECT a2 FROM a
            GO

            ALTER VIEW c (c2) AS SELECT b2 FROM b
            """;

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }
}
