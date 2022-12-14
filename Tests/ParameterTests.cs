using Mast.Dbo;

namespace Tests;

internal class ParameterTests : BaseMastTest
{
    private Random rand = new();

    [Test]
    public void Content()
    {
        var expected = "@SomeParam int not null";
        var function = $"CREATE FUNCTION dbo.stub({expected}) RETURNS TABLE AS RETURN SELECT 1";

        parser.Parse(db, function);
        var result = db.Functions.First().Parameters.First();

        Assert.That(result.Content, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("INT")]
    [TestCase("dbo.INT")]
    [TestCase("NVARCHAR(max)")]
    public void DataType(string expected)
    {
        var function = $"CREATE FUNCTION dbo.stub(@Stub {expected}) RETURNS TABLE AS RETURN SELECT 1";

        parser.Parse(db, function);
        var result = db.Functions.First().Parameters.First();

        Assert.That(result.DataType.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public void DefaultValue()
    {
        var expected = rand.Next().ToString();
        var function = $"CREATE FUNCTION dbo.stub(@Stub int = {expected}) RETURNS TABLE AS RETURN SELECT 1";

        parser.Parse(db, function);
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
        var function = $"CREATE Procedure dbo.stub @Stub int{mod} AS RETURN SELECT 1";

        parser.Parse(db, function);
        var result = db.Procedures.First().Parameters.First();

        Assert.That(result.Modifier, Is.EqualTo(expected));
    }

    [Test]
    public void Name()
    {
        var expected = "@Slartibartfast";
        var function = $"CREATE FUNCTION dbo.stub({expected} int) RETURNS TABLE AS RETURN SELECT 1";

        parser.Parse(db, function);
        var result = db.Functions.First().Parameters.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", null)]
    [TestCase(" NULL", true)]
    [TestCase(" NOT NULL", false)]
    public void Nullability(string nullSpecifier, bool? nullable)
    {
        var function = $"CREATE FUNCTION dbo.stub(@stub int{nullSpecifier}) RETURNS TABLE AS RETURN SELECT 1";

        parser.Parse(db, function);
        var result = db.Functions.First().Parameters.First();

        Assert.That(result.IsNullable, Is.EqualTo(nullable));
    }
}
