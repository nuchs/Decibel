namespace Tests.Patchwerk;

internal class TypePatchTests : BasePatchwerkTest
{
    [Test]
    [TestCase("CREATE TYPE scalar FROM INT")]
    [TestCase("CREATE TYPE tuble AS TABLE (a INT)")]
    public void Add(string expected)
    {
        var before = MakeDb();
        var after = MakeDb(expected);

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("FROM INT", "FROM CHAR")]
    [TestCase("AS TABLE (a INT)", "AS TABLE (a INT, b INT)")]
    public void ChangeContent(string oldDef, string newDef)
    {
        var before = MakeDb($"CREATE TYPE mytype {oldDef}");
        var after = MakeDb($"CREATE TYPE mytype {newDef}");
        var expected = $"""
            DROP TYPE mytype
            GO

            CREATE TYPE mytype {newDef}
            """;

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("FROM INT")]
    [TestCase("AS TABLE (a INT)")]
    public void NoChange(string definition)
    {
        var before = MakeDb($"CREATE TYPE mytype {definition}");
        var after = MakeDb($"CREATE TYPE mytype {definition}");

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.Empty);
    }

    [Test]
    [TestCase("FROM INT")]
    [TestCase("AS TABLE (a INT)")]
    public void Remove(string definition)
    {
        var before = MakeDb($"CREATE TYPE mytype {definition}");
        var after = MakeDb();
        var expected = "DROP TYPE mytype";

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("FROM INT", "FROM CHAR")]
    [TestCase("AS TABLE (a INT)", "AS TABLE (a INT, b INT)")]
    public void CannotUpdateTypeUsedInTable(string oldDef, string newDef)
    {
        var before = MakeDb(
            $"CREATE TYPE mytype {oldDef}",
            "CREATE TABLE tuble (a mytype)");
        var after = MakeDb(
            $"CREATE TYPE mytype {newDef}",
            "CREATE TABLE tuble (a mytype)");

        Assert.Throws<InvalidOperationException>(() => sut.GeneratePatches(before, after));
    }

    [Test]
    [TestCase("FROM INT", "FROM CHAR")]
    [TestCase("AS TABLE (a INT)", "AS TABLE (a INT, b INT)")]
    public void SimpleDepend(string oldDef, string newDef)
    {
        var before = MakeDb(
           $"CREATE TYPE mytype {oldDef}",
           "CREATE FUNCTION func(@a mytype READONLY) RETURNS INT AS BEGIN RETURN 1 END");
        var after = MakeDb(
            $"CREATE TYPE mytype {newDef}",
            "CREATE FUNCTION func(@a mytype READONLY) RETURNS INT AS BEGIN RETURN 1 END");
        var expected = $"""
            DROP FUNCTION func
            GO

            DROP TYPE mytype
            GO

            CREATE TYPE mytype {newDef}
            GO

            CREATE FUNCTION func(@a mytype READONLY) RETURNS INT AS BEGIN RETURN 1 END
            """;

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void SharedDepend()
    {
        var before = MakeDb(
            $"CREATE TYPE type1 FROM INT",
            $"CREATE TYPE type2 FROM INT",
            "CREATE FUNCTION func(@a type1, @b type2) RETURNS INT AS BEGIN RETURN 1 END");
        var after = MakeDb(
            $"CREATE TYPE type1 FROM CHAR",
            $"CREATE TYPE type2 FROM CHAR",
            "CREATE FUNCTION func(@a type1, @b type2) RETURNS INT AS BEGIN RETURN 1 END");
        var expected = $"""
            DROP FUNCTION func
            GO

            DROP TYPE type2
            GO

            DROP TYPE type1
            GO

            CREATE TYPE type1 FROM CHAR
            GO

            CREATE TYPE type2 FROM CHAR
            GO

            CREATE FUNCTION func(@a type1, @b type2) RETURNS INT AS BEGIN RETURN 1 END
            """;

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void TransitiveDepend()
    {
        var before = MakeDb(
            $"CREATE TYPE type2 FROM type1",
            $"CREATE TYPE type3 FROM type2",
            $"CREATE TYPE type1 FROM INT");
        var after = MakeDb(
            $"CREATE TYPE type3 FROM type2",
            $"CREATE TYPE type1 FROM CHAR",
            $"CREATE TYPE type2 FROM type1");
        var expected = $"""
            DROP TYPE type3
            GO

            DROP TYPE type2
            GO

            DROP TYPE type1
            GO

            CREATE TYPE type1 FROM CHAR
            GO

            CREATE TYPE type2 FROM type1
            GO

            CREATE TYPE type3 FROM type2
            """;

        var result = sut.GeneratePatches(before, after);

        Assert.That(result, Is.EqualTo(expected));
    }
}
