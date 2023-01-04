using Lindt;
using Mast;

namespace Tests.Lindt;

internal class UnreferencedTests
{
    Linter sut = new();

    [SetUp]
    public void SetUp()
    {
        sut = new();
        sut["Unresolved"] = false;
    }

    [Test]
    public void ProceduresAreNotChecked()
    {
        var db = new DbBuilder().AddFromTsqlScript("CREATE PROCEDURE bob AS BEGIN SELECT 1 END").Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }

    [Test]
    public void UsersAreNotChecked()
    {
        var db = new DbBuilder().AddFromTsqlScript("CREATE USER bob").Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }

    [Test]
    public void TriggersAreNotChecked()
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
    public void ReferencedTables()
    {
        var script = """
            CREATE TABLE bob (col int)
            GO
            CREATE PROCEDURE pob AS BEGIN SELECT * FROM bob END;
            """;
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }

    [Test]
    public void ReferencedSchemas()
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
    public void ReferencedTableTypes()
    {
        var script = """
            CREATE TYPE bob AS TABLE (col int)
            GO
            CREATE PROCEDURE pob @arg bob READONLY AS BEGIN SELECT * FROM @arg END;
            """;
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }

    [Test]
    public void ReferencedScalarTypes()
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
    public void ReferencedFunctions()
    {
        var script = """
            CREATE FUNCTION bob() RETURNS TABLE RETURN SELECT 1
            GO
            CREATE PROCEDURE bob.pob AS BEGIN SELECT bob() END;
            """;
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }

    [Test]
    public void ReferencedViews()
    {
        var script = """
            CREATE TABLE bob (col int)
            GO
            CREATE VIEW vob AS SELECT bob.col
            GO
            CREATE PROCEDURE pob AS BEGIN SELECT * FROM vob END;
            """;
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }

    [Test]
    public void UnreferencedTables()
    {
        var tableName = "bob";
        var script = $"CREATE TABLE {tableName} (col int)";
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var result = sut.Run(db).Results.First();

        Assert.That(result.CheckName, Is.EqualTo("Unreferenced"));
        Assert.That(result.Description, Contains.Substring(tableName));
    }

    [Test]
    public void UnreferencedSchemas()
    {
        var schemaName = "bob";
        var script = $"CREATE SCHEMA {schemaName}";
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var result = sut.Run(db).Results.First();

        Assert.That(result.CheckName, Is.EqualTo("Unreferenced"));
        Assert.That(result.Description, Contains.Substring(schemaName));
    }

    [Test]
    public void UnreferencedTableTypes()
    {
        var typeName = "bob";
        var script = $"CREATE TYPE {typeName} AS TABLE (col int)";
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var result = sut.Run(db).Results.First();

        Assert.That(result.CheckName, Is.EqualTo("Unreferenced"));
        Assert.That(result.Description, Contains.Substring(typeName));
    }

    [Test]
    public void UnreferencedScalarTypes()
    {
        var typeName = "bob";
        var script = $"CREATE TYPE {typeName} FROM INT";
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var result = sut.Run(db).Results.First();

        Assert.That(result.CheckName, Is.EqualTo("Unreferenced"));
        Assert.That(result.Description, Contains.Substring(typeName));
    }

    [Test]
    public void UnreferencedFunctions()
    {
        var funcName = "bob";
        var script = $"CREATE FUNCTION {funcName} () RETURNS TABLE AS RETURN SELECT 1";
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var result = sut.Run(db).Results.First();

        Assert.That(result.CheckName, Is.EqualTo("Unreferenced"));
        Assert.That(result.Description, Contains.Substring(funcName));
    }

    [Test]
    public void UnreferencedViews()
    {
        var viewName = "bob";
        var script = $"CREATE VIEW {viewName} (col) AS SELECT 1";
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var result = sut.Run(db).Results.First();

        Assert.That(result.CheckName, Is.EqualTo("Unreferenced"));
        Assert.That(result.Description, Contains.Substring(viewName));
    }
}
