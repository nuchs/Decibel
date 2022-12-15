using Mast.Parsing;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class ScalarType : DbObject
{
    public ScalarType(CreateTypeUddtStatement node)
        : base(node)
    {
        Identifier = AssembleIdentifier(node.Name);
        IsNullable = GetNullability(node);
        Parameters = CollectParameters(node.DataType);
    }

    public bool? IsNullable { get; }

    public IEnumerable<string> Parameters { get; }

    private static IEnumerable<string> CollectParameters(DataTypeReference dataTypeRef)
        => dataTypeRef is SqlDataTypeReference sqlRef ?
            sqlRef.Parameters.Select(p => p.Value) :
            new List<string>();

    private protected override (IEnumerable<DbObject>, IEnumerable<FullyQualifiedName>) GetReferents(Database db) 
        => CorralateRefs(db.Schemas, new FullyQualifiedName(string.Empty, Identifier.Schema));

    private FullyQualifiedName AssembleIdentifier(SchemaObjectName node)
        => new(GetId(node.SchemaIdentifier), GetId(node.BaseIdentifier));

    private bool GetNullability(CreateTypeUddtStatement node)
        => node.NullableConstraint is null || node.NullableConstraint.Nullable;
}
