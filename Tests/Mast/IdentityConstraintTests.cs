namespace Tests.Mast;

public class IdentityConstraintTests : BaseMastTest
{
    [Test]
    public void BareIdentityIncrement()
    {
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub int identity)";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Identity?.Increment, Is.EqualTo(1));
    }

    [Test]
    public void BareIdentitySeed()
    {
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub int identity)";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Identity?.Seed, Is.EqualTo(1));
    }

    [Test]
    [TestCase("identity")]
    [TestCase("identity(1,2)")]
    public void Content(string expected)
    {
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub int {expected})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Identity?.Content, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(1)]
    [TestCase(2)]
    public void IdentityIncrement(int expected)
    {
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub int identity(0, {expected}))";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Identity?.Increment, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(1)]
    public void IdentitySeed(int expected)
    {
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub int identity({expected}, 1))";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Columns.First();

        Assert.That(result.Identity?.Seed, Is.EqualTo(expected));
    }
}
