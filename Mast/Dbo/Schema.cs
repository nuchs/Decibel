using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class Schema : DbObject
{
    public Schema(CreateSchemaStatement schema)
        : base(schema)
    {
        Name = GetName(schema);
        Owner = GetOwner(schema);
    }

    public string Owner { get; }

    private string GetName(CreateSchemaStatement schema)
        => GetId(schema.Name);

    private string GetOwner(CreateSchemaStatement schema)
                => GetId(schema.Owner);
}
