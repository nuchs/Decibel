using Mast.Parsing;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class Schema : DbObject
{
    public Schema(CreateSchemaStatement schema)
    : base(schema)
    {
        Identifier = GetSchema(schema);
        Owner = GetOwner(schema);
    }

    public string Owner { get; }

    private FullyQualifiedName GetSchema(CreateSchemaStatement schema)
        => FullyQualifiedName.FromSchema(GetId(schema.Name));

    private string GetOwner(CreateSchemaStatement schema)
        => GetId(schema.Owner);
}
