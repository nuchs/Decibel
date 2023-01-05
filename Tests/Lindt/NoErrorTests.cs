using Lindt;
using Mast;

namespace Tests.Lindt;

public class NoErrorTests
{
    private Linter sut = new();

    [Test]
    public void ValidFunctions()
    {
        var script = """
            CREATE FUNCTION bob() RETURNS TABLE RETURN SELECT 1
            GO
            CREATE PROCEDURE pob AS BEGIN SELECT bob() END;
            """;
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }

    [Test]
    public void ValidProcedures()
    {
        var script = "CREATE PROCEDURE pob AS BEGIN SELECT 1 END;";
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }

    [Test]
    public void ValidScalarTypes()
    {
        var script = """
            CREATE TYPE bob FROM INT
            GO
            CREATE PROCEDURE pob @arg bob AS BEGIN SELECT @arg END;
            """;
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }

    [Test]
    public void ValidSchemas()
    {
        var script = """
            CREATE SCHEMA bob
            GO
            CREATE PROCEDURE bob.pob AS BEGIN SELECT 1 END;
            """;
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }

    [Test]
    public void ValidTables()
    {
        var script = """
            CREATE TABLE bob (col int)
            GO
            CREATE PROCEDURE pob AS BEGIN SELECT b.col FROM bob b END;
            """;
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }

    [Test]
    public void ValidTableTypes()
    {
        var script = """
            CREATE TYPE bob AS TABLE (col int)
            GO
            CREATE PROCEDURE pob @arg bob READONLY AS BEGIN SELECT a.col FROM @arg a END;
            """;
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }

    [Test]
    public void ValidTrigger()
    {
        var script = """
            CREATE TABLE bob (col int)
            GO
            CREATE TRIGGER tigger on bob after insert as select 1
            """;
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }

    [Test]
    public void ValidViews()
    {
        var script = """
            CREATE TABLE bob (col int)
            GO
            CREATE VIEW vob (vcol) AS SELECT bob.col from bob
            GO
            CREATE PROCEDURE pob AS BEGIN SELECT v.vcol FROM vob v END;
            """;
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }
}
