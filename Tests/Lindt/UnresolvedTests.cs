using Lindt;
using Mast;

namespace Tests.Lindt;

public class UnresolvedTests
{
    private Linter sut = new();

    [SetUp]
    public void SetUp()
    {
        sut = new();
        sut["Unreferenced"] = false;
    }

    [Test]
    public void UnresolvedFunction()
    {
        var symbol = "somefunc";
        var script = $"CREATE PROCEDURE stub AS BEGIN select {symbol}() END";
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var result = sut.Run(db).Results.First();

        Assert.That(result.CheckName, Is.EqualTo("Unresolved"));
        Assert.That(result.Description, Contains.Substring(symbol));
    }

    [Test]
    public void UnresolvedScalarType()
    {
        var symbol = "sometype";
        var script = $"CREATE PROCEDURE stub @arg {symbol} AS BEGIN select 1 END";
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var result = sut.Run(db).Results.First();

        Assert.That(result.CheckName, Is.EqualTo("Unresolved"));
        Assert.That(result.Description, Contains.Substring(symbol));
    }

    [Test]
    public void UnresolvedSchema()
    {
        var symbol = "someschema";
        var script = $"CREATE PROCEDURE {symbol}.stub AS BEGIN select 1 END";
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var result = sut.Run(db).Results.First();

        Assert.That(result.CheckName, Is.EqualTo("Unresolved"));
        Assert.That(result.Description, Contains.Substring(symbol));
    }

    [Test]
    public void UnresolvedTable()
    {
        var symbol = "sometable";
        var script = $"CREATE PROCEDURE stub AS BEGIN select * from {symbol} END";
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var result = sut.Run(db).Results.First();

        Assert.That(result.CheckName, Is.EqualTo("Unresolved"));
        Assert.That(result.Description, Contains.Substring(symbol));
    }

    [Test]
    [TestCase("b.col")]
    [TestCase("col1")]
    [TestCase("sometable.col1")]
    public void UnresolvedTableColumn(string symbol)
    {
        var script = $"""
            CREATE TABLE someTable (col int)
            GO

            CREATE PROCEDURE stub AS BEGIN select {symbol} from someTable a END
            """;
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var results = sut.Run(db).Results;

        Assert.That(results.Select(r => r.CheckName), Has.All.Contains("Unresolved") );
        Assert.That(results.Select(r => r.Description), Has.Some.Contain(symbol));
    }

    [Test]
    public void UnresolvedTableType()
    {
        var symbol = "sometype";
        var script = $"CREATE PROCEDURE stub @arg {symbol} READONLY AS BEGIN select * from @arg END";
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var result = sut.Run(db).Results.First();

        Assert.That(result.CheckName, Is.EqualTo("Unresolved"));
        Assert.That(result.Description, Contains.Substring(symbol));
    }

    [Test]
    [TestCase("b.col")]
    [TestCase("col1")]
    public void UnresolvedTableTypeColumn(string symbol)
    {
        var script = $"""
            CREATE TYPE someType AS TABLE (col int)
            GO

            CREATE PROCEDURE stub @arg someType READONLY AS BEGIN select {symbol} from @arg a END
            """;
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var results = sut.Run(db).Results;

        Assert.That(results.Select(r => r.CheckName), Has.All.Contains("Unresolved"));
        Assert.That(results.Select(r => r.Description), Has.Some.Contain(symbol));
    }

    [Test]
    public void UnresolvedForeignKeyColumnConstraint()
    {
        var expected = "bacon";
        var script = $"""
            CREATE TABLE tab1 (col1 int)
            GO

            CREATE TABLE tab2 (col1 int references tab1 ({expected})) 
            """;
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var results = sut.Run(db).Results;

        Assert.That(results.Select(r => r.CheckName), Has.All.Contains("Unresolved"));
        Assert.That(results.Select(r => r.Description), Has.Some.Contain(expected));
    }

    [Test]
    public void UnresolvedForeignKeyTableConstraint()
    {
        var expected = "bacon";
        var script = $"""
            CREATE TABLE tab1 (col1 int)
            GO

            CREATE TABLE tab2 (col1 int, foreign key (col1) references tab1 ({expected})) 
            """;
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var results = sut.Run(db).Results;

        Assert.That(results.Select(r => r.CheckName), Has.All.Contains("Unresolved"));
        Assert.That(results.Select(r => r.Description), Has.Some.Contain(expected));
    }
}
