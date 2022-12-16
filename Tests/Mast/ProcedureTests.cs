using Mast.Dbo;

namespace Tests.Mast;

public class ProcedureTests : BaseMastTest
{
    [Test]
    public void Content()
    {
        var script = """
            CREATE PROCEDURE dbo.stub
            AS
            BEGIN
                DECLARE @val int;
                set @val = 42;
                SELECT @val
            END
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Procedures.First();

        Assert.That(result.Content, Is.EqualTo(script.Trim()));
    }

    [Test]
    [TestCase("bear", "bear", "bear", "bear")]
    [TestCase("bear", "[bracketed]", "bear", "bracketed")]
    [TestCase("[bracketed]", "bear", "bracketed", "bear")]
    [TestCase("[bracketed]", "[bracketed]", "bracketed", "bracketed")]
    public void Identifier(string name, string schema, string bareName, string bareSchema)
    {
        var expected = FullyQualifiedName.FromSchemaName(bareSchema, bareName);
        var script = $"CREATE PROCEDURE {schema}.{name} AS SELECT 1";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Procedures.First();

        Assert.That(result.Identifier, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", 0)]
    [TestCase("@a int out", 1)]
    [TestCase("@a int, @b int, @c int", 3)]
    public void ParameterCount(string paramList, int expected)
    {
        var script = $"""
            CREATE PROCEDURE dbo.stub
            {paramList}
            AS
            BEGIN
                RETURN SELECT 1
            END
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Procedures.First();

        Assert.That(result.Parameters, Has.Exactly(expected).Items);
    }

    [Test]
    [TestCase("dbo")]
    [TestCase("[dbo]")]
    public void ReferenceSchema(string schemaName)
    {
        var script = $"""
            CREATE SCHEMA {schemaName}
            GO

            CREATE procedure {schemaName}.stub AS SELECT 1
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var proc = db.Procedures.First();
        var schema = db.Schemas.First();

        Assert.That(schema.ReferencedBy, Has.Member(proc));
    }

    [Test]
    public void ReferenceScalar()
    {
        var type = "my.type";
        var script = $"""
            CREATE TYPE {type} FROM INT
            GO

            CREATE procedure dbo.stub @a {type} AS SELECT 1
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var proc = db.Procedures.First();
        var scalar = db.ScalarTypes.First();

        Assert.That(scalar.ReferencedBy, Has.Member(proc));
    }

    [Test]
    public void ReferenceTableType()
    {
        var type = "my.type";
        var script = $"""
            CREATE TYPE {type} as TABLE(col int)
            GO

            CREATE procedure dbo.stub @a {type} READONLY AS SELECT 1
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var proc = db.Procedures.First();
        var tt = db.TableTypes.First();

        Assert.That(tt.ReferencedBy, Has.Member(proc));
    }
}
