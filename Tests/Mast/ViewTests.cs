namespace Tests.Mast;

public class ViewTests : BaseMastTest
{
    [Test]
    [TestCase("", false)]
    [TestCase(" WITH CHECK OPTION", true)]
    public void CheckOption(string check, bool expected)
    {
        var script = $"CREATE View dbo.stub (col) AS select tab.a{check}";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Views.First();

        Assert.That(result.Check, Is.EqualTo(expected));
    }

    [Test]
    public void Columns()
    {
        var script = "CREATE View dbo.stub (col1, col2) AS select tab.a, tab2.b";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Views.First();

        Assert.That(result.Columns, Is.EquivalentTo(new[] { "col1", "col2" }));
    }

    [Test]
    public void Content()
    {
        var script = $"CREATE View dbo.stub (col1, col2) AS select tab.a, tab.b";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Views.First();

        Assert.That(result.Content, Is.EqualTo(script));
    }

    [Test]
    public void Name()
    {
        var expected = "bob";
        var script = $"CREATE View dbo.{expected} (col) AS select tab.a";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Views.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    public void Schema()
    {
        var expected = "dbo";
        var script = $"CREATE View {expected}.stub (col) AS select tab.a";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Views.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", false)]
    [TestCase(" WITH SCHEMABINDING ", true)]
    public void SchemaBinding(string binding, bool expected)
    {
        var script = $"CREATE View dbo.stub (col) {binding} AS select tab.a";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Views.First();

        Assert.That(result.SchemaBinding, Is.EqualTo(expected));
    }
}
