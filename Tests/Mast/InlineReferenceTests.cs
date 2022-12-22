using Mast.Dbo;
using Mast.Parsing;

namespace Tests.Mast;

internal class InlineReferenceTests : BaseMastTest
{
    [Test]
    public void FunctionReference()
    {
        var script = """
            CREATE FUNCTION dbo.func1() RETURNS TABLE AS RETURN SELECT 1
            GO

            CREATE FUNCTION dbo.func2() RETURNS INT AS BEGIN RETURN dbo.func1() END
            GO
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var f1 = db.Functions.First(f => f.Identifier == FullyQualifiedName.FromSchemaName("dbo", "func1"));
        var f2 = db.Functions.First(f => f.Identifier == FullyQualifiedName.FromSchemaName("dbo", "func2"));

        Assert.That(f1.ReferencedBy, Has.Member(f2));
    }

    [Test]
    public void ProcedureReference()
    {
        var script = """
            CREATE PROCEDURE Proc1 AS BEGIN SELECT 1 END
            GO

            CREATE PROCEDURE dbo.Proc2 AS BEGIN EXEC Proc1 END
            GO
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var p1 = db.Procedures.First(p => p.Identifier == FullyQualifiedName.FromName("Proc1"));
        var p2 = db.Procedures.First(p => p.Identifier == FullyQualifiedName.FromSchemaName("dbo", "Proc2"));

        Assert.That(p1.ReferencedBy, Has.Member(p2));
    }

    [Test]
    public void ViewReference()
    {
        var script = """
            CREATE View dbo.stub (col) AS select tab.a
            GO

            CREATE PROCEDURE dbo.Proc1 AS BEGIN SELECT * from dbo.stub END
            GO
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var proc = db.Procedures.First(p => p.Identifier == FullyQualifiedName.FromSchemaName("dbo", "PROC1"));
        var view = db.Views.First(p => p.Identifier == FullyQualifiedName.FromSchemaName("dbo", "Stub"));

        Assert.That(view.ReferencedBy, Has.Member(proc));
    }


    [Test]
    public void UnresolvedSchemaReference()
    {
        var schema = "Liono";
        var script = $"CREATE TYPE {schema}.stub FROM INT";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var expected = new Reference(db.ScalarTypes.First(), FullyQualifiedName.FromSchema(schema));

        Assert.That(db.UnresolvedReferences, Has.Member(expected));
    }
}
