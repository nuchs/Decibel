using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Xml.Linq;

namespace Mast.Dbo;

public sealed class Schema : DbObject
{
    public Schema(CreateSchemaStatement schema)
    : base(schema)
    {
        Identifier = new(string.Empty, GetName(schema));
        Owner = GetOwner(schema);
    }

    public string Owner { get; }

    private string GetName(CreateSchemaStatement schema)
        => GetId(schema.Name);

    private string GetOwner(CreateSchemaStatement schema)
        => GetId(schema.Owner);
}
