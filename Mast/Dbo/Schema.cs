using Mast.Parsing;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class Schema : DbObject
{
    public Schema(CreateSchemaStatement schema)
        : base(schema)
    {
        Name = GetName(schema);
        Owner = GetOwner(schema);
    }

    public string Owner { get; }

    internal override void CrossReference(Database db) => throw new NotImplementedException();
    private string GetName(CreateSchemaStatement schema)
        => GetId(schema.Name);

    private string GetOwner(CreateSchemaStatement schema)
                => GetId(schema.Owner);
}
