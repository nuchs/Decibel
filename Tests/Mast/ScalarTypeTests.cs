using Mast.Dbo;
using Mast.Parsing;

namespace Tests.Mast;

public class ScalarTypeTests : BaseMastTest
{
    [Test]
    public void Content()
    {
        var script = "CREATE TYPE dbo.stub FROM int";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.ScalarTypes.First();

        Assert.That(result.Content, Is.EqualTo(script));
    }

    [Test]
    [TestCase("bear", "bear", "bear", "bear")]
    [TestCase("bear", "[bracketed]", "bear", "bracketed")]
    [TestCase("[bracketed]", "bear", "bracketed", "bear")]
    [TestCase("[bracketed]", "[bracketed]", "bracketed", "bracketed")]
    public void Identifier(string name, string schema, string bareName, string bareSchema)
    {
        FullyQualifiedName expected = new(bareSchema, bareName);
        var script = $"CREATE TYPE {schema}.{name} FROM INT";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.ScalarTypes.First();

        Assert.That(result.Identifier, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", true)]
    [TestCase(" NULL", true)]
    [TestCase(" NOT NULL", false)]
    public void Nullability(string nullSpecifier, bool nullable)
    {
        var script = $"CREATE TYPE dbo.stub FROM INT{nullSpecifier}";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.ScalarTypes.First();

        Assert.That(result.IsNullable, Is.EqualTo(nullable));
    }

    [Test]
    [TestCase("1")]
    [TestCase("200")]
    [TestCase("max")]
    public void ParameterisedType(string expected)
    {
        var script = $"CREATE TYPE dbo.stub FROM NVARCHAR({expected})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.ScalarTypes.First();

        Assert.That(result.Parameters, Is.EquivalentTo(new List<string> { expected }));
    }

    [Test]
    [TestCase("dbo")]
    [TestCase("[dbo]")]
    public void ReferenceSchema(string schemaName)
    {
        var script = $"""
            CREATE SCHEMA {schemaName}
            GO

            CREATE TYPE {schemaName}.stub FROM INT
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var scalar = db.ScalarTypes.First();
        var schema = db.Schemas.First();

        Assert.That(schema.ReferencedBy, Has.Member(scalar));
    }

    [Test]
    public void UnparameterisedType()
    {
        var script = $"CREATE TYPE dbo.stub FROM INT";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.ScalarTypes.First();

        Assert.That(result.Parameters, Is.Empty);
    }

    [Test]
    public void UnreferencedSchema()
    {
        var script = $"""
            CREATE SCHEMA Liono
            GO

            CREATE TYPE Cheetahra.stub FROM INT
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var schema = db.Schemas.First();

        Assert.That(schema.ReferencedBy, Is.Empty);
    }

    [Test]
    public void UnresolvedSchemaReference()
    {
        var schema = "Liono";
        var script = $"CREATE TYPE {schema}.stub FROM INT";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var expected = new Reference(db.ScalarTypes.First(), schema);

        Assert.That(db.UnresolvedReferences, Has.Member(expected));
    }
}
