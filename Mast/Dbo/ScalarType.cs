using Mast.Parsing;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Collections.Generic;

namespace Mast.Dbo;

public sealed class ScalarType : DbObject
{
    public ScalarType(DataTypeReference dataTypeRef)
        : base(dataTypeRef)
    {
        Name = GetName(dataTypeRef.Name);
        Schema = GetSchema(dataTypeRef.Name);
        Parameters = CollectParameters(dataTypeRef);
    }

    public ScalarType(CreateTypeUddtStatement node)
        : base(node)
    {
        Name = GetName(node.Name);
        Schema = GetSchema(node.Name);
        IsNullable = GetNullability(node);
        Parameters = CollectParameters(node.DataType);
    }

    public bool? IsNullable { get; }

    public IEnumerable<string> Parameters { get; }

    public string Schema { get; }

    private static IEnumerable<string> CollectParameters(DataTypeReference dataTypeRef)
        => dataTypeRef is SqlDataTypeReference sqlRef ?
            sqlRef.Parameters.Select(p => p.Value) :
            new List<string>();

    private protected override (IEnumerable<DbObject>, IEnumerable<string>) GetReferents(Database db)
    {
        List<string> unresolved = new();
        var referents = db.Schemas.Where(s => s.Name == Schema);

        if (!referents.Any())
        {
            unresolved.Add(Schema);
        }

        return (referents, unresolved);
    }

    private string GetName(SchemaObjectName fullyQualifiedName)
        => GetId(fullyQualifiedName.BaseIdentifier);

    private bool GetNullability(CreateTypeUddtStatement node)
        => node.NullableConstraint is null || node.NullableConstraint.Nullable;

    private string GetSchema(SchemaObjectName fullyQualifiedName)
        => GetId(fullyQualifiedName.SchemaIdentifier);
}
