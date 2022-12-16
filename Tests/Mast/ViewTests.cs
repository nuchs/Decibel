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
        var script = "CREATE View dbo.stub (col1, col2) AS select t1.a, t2.b from tab1 t1, tab2 t2";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Views.First();

        Assert.That(result.Columns, Is.EquivalentTo(new[] { "col1", "col2" }));
    }

    [Test]
    public void BaseTables()
    {
        var tab1 = FullyQualifiedName.FromSchemaName("dbo", "tom");
        var tab2 = FullyQualifiedName.FromName("dick");
        var tab3 = new FullyQualifiedName("db", "dbo", "Harry");
        var script = $"CREATE View dbo.stub (c1, c2, c3) AS select t1.a, t2.b, t3.c from {tab1} t1, {tab2} t2, {tab3}";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Views.First();

        Assert.That(result.BaseTables, Is.EquivalentTo(new[] { tab1, tab2, tab3 }));
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

    [Test]
    [TestCase("dbo")]
    [TestCase("[dbo]")]
    public void ReferenceSchema(string schemaName)
    {
        var script = $"""
            CREATE SCHEMA {schemaName}
            GO

            CREATE view {schemaName}.stub (stub) as select dbo2.tab.a
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var view = db.Views.First();
        var schema = db.Schemas.First();

        Assert.That(schema.ReferencedBy, Has.Member(view));
    }

    [Test]
    public void ReferenceTable ()
    {
        var script = $"""
            CREATE TABLE dbo.jim (col int)
            GO

            CREATE view dbo.stub (stub) as select col from dbo.jim
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var view = db.Views.First();
        var table = db.Tables.First();

        Assert.That(table.ReferencedBy, Has.Member(view));
    }
}
