using Lindt;
using Mast;

namespace Tests.Lindt;

internal class UnreferencedTests
{
    private Linter sut = new();

    [SetUp]
    public void SetUp()
    {
        sut = new();
        sut["Unresolved"] = false;
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
    public void UnreferencedViews()
    {
        var viewName = "bob";
        var script = $"CREATE VIEW {viewName} (col) AS SELECT 1";
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var result = sut.Run(db).Results.First();

        Assert.That(result.CheckName, Is.EqualTo("Unreferenced"));
        Assert.That(result.Description, Contains.Substring(viewName));
    }

    [Test]
    public void UsersAreNotChecked()
    {
        var db = new DbBuilder().AddFromTsqlScript("CREATE USER bob").Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }
}
