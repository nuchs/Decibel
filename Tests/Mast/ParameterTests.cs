using Mast.Dbo;

namespace Tests.Mast;

public class ParameterTests : BaseMastTest
{
    private Random rand = new();

    [Test]
    public void Content()
    {
        var expected = "@SomeParam int not null";
        var script = $"CREATE FUNCTION dbo.stub({expected}) RETURNS TABLE AS RETURN SELECT 1";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Functions.First().Parameters.First();

        Assert.That(result.Content, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("int", "", "int")]
    [TestCase("nvarchar(max)", "", "nvarchar")]
    [TestCase("MySchema.MyType", "MySchema", "MyType")]
    public void DataType(string type, string schema, string name)
    {
        FullyQualifiedName expected = new(schema, name);
        var script = $"CREATE FUNCTION dbo.stub(@Stub {type}) RETURNS TABLE AS RETURN SELECT 1";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Functions.First().Parameters.First();

        Assert.That(result.DataType, Is.EqualTo(expected));
    }

    [Test]
    public void DefaultValue()
    {
        var expected = rand.Next().ToString();
        var script = $"CREATE FUNCTION dbo.stub(@Stub int = {expected}) RETURNS TABLE AS RETURN SELECT 1";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Functions.First().Parameters.First();

        Assert.That(result.Default, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", ParameterMod.None)]
    [TestCase(" OUT", ParameterMod.Output)]
    [TestCase(" OUTPUT", ParameterMod.Output)]
    [TestCase(" READONLY", ParameterMod.Readonly)]
    public void Modifier(string mod, ParameterMod expected)
    {
        var script = $"CREATE Procedure dbo.stub @Stub int{mod} AS RETURN SELECT 1";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Procedures.First().Parameters.First();

        Assert.That(result.Modifier, Is.EqualTo(expected));
    }

    [Test]
    public void Name()
    {
        var expected = "@Slartibartfast";
        var script = $"CREATE FUNCTION dbo.stub({expected} int) RETURNS TABLE AS RETURN SELECT 1";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Functions.First().Parameters.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", null)]
    [TestCase(" NULL", true)]
    [TestCase(" NOT NULL", false)]
    public void Nullability(string nullSpecifier, bool? nullable)
    {
        var script = $"CREATE FUNCTION dbo.stub(@stub int{nullSpecifier}) RETURNS TABLE AS RETURN SELECT 1";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Functions.First().Parameters.First();

        Assert.That(result.IsNullable, Is.EqualTo(nullable));
    }
}
