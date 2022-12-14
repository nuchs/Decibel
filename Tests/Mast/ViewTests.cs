namespace Tests.Mast;

public class ViewTests : BaseMastTest
{
    [Test]
    [TestCase("", false)]
    [TestCase(" WITH CHECK OPTION", true)]
    public void CheckOption(string check, bool expected)
    {
        var view = $"CREATE View dbo.stub (col) AS select tab.a{check}";

        parser.Parse(db, view);
        var result = db.Views.First();

        Assert.That(result.Check, Is.EqualTo(expected));
    }

    [Test]
    public void Columns()
    {
        var view = "CREATE View dbo.stub (col1, col2) AS select tab.a, tab.b";

        parser.Parse(db, view);
        var result = db.Views.First();

        Assert.That(result.Columns, Is.EquivalentTo(new[] { "col1", "col2" }));
    }

    [Test]
    public void Content()
    {
        var expected = $"CREATE View dbo.stub (col1, col2) AS select tab.a, tab.b";

        parser.Parse(db, expected);
        var result = db.Views.First();

        Assert.That(result.Content, Is.EqualTo(expected));
    }

    [Test]
    public void Name()
    {
        var expected = "bob";
        var view = $"CREATE View dbo.{expected} (col) AS select tab.a";

        parser.Parse(db, view);
        var result = db.Views.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    public void Schema()
    {
        var expected = "dbo";
        var view = $"CREATE View {expected}.stub (col) AS select tab.a";

        parser.Parse(db, view);
        var result = db.Views.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", false)]
    [TestCase(" WITH SCHEMABINDING ", true)]
    public void SchemaBinding(string binding, bool expected)
    {
        var view = $"CREATE View dbo.stub (col) {binding} AS select tab.a";

        parser.Parse(db, view);
        var result = db.Views.First();

        Assert.That(result.SchemaBinding, Is.EqualTo(expected));
    }
}
