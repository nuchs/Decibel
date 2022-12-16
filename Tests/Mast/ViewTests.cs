using Mast.Dbo;

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
    [TestCase("bear", "bear", "bear", "bear")]
    [TestCase("bear", "[bracketed]", "bear", "bracketed")]
    [TestCase("[bracketed]", "bear", "bracketed", "bear")]
    [TestCase("[bracketed]", "[bracketed]", "bracketed", "bracketed")]
    public void Identifier(string name, string schema, string bareName, string bareSchema)
    {
        var expected = FullyQualifiedName.FromSchemaName(bareSchema, bareName);
        var script = $"CREATE View {schema}.{name} (col) AS select tab.a";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Views.First();

        Assert.That(result.Identifier, Is.EqualTo(expected));
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
