using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class ScalarType
{
    public ScalarType(DataTypeReference dataTypeRef)
    {
        Name = GetName(dataTypeRef.Name);
        Schema = GetSchema(dataTypeRef.Name);
        Parameters = CollectParameters(dataTypeRef);
    }

    public ScalarType(CreateTypeUddtStatement node)
    {
        Name = GetName(node.Name);
        Schema = GetSchema(node.Name);
        IsNullable = GetNullability(node);
        Parameters = CollectParameters(node.DataType);
    }

    public override string ToString()
    {
        var schemaPart = string.IsNullOrWhiteSpace(Schema) ? string.Empty : $"{Schema}."; 
        var parameters = string.Join(", ", Parameters);
        var parameterList = parameters == string.Empty ? string.Empty : $"({parameters})";
        var nullSepc = IsNullable is null ? string.Empty :
                       IsNullable.Value   ? " NULL" : " NOT NULL";
        return $"{schemaPart}{Name}{parameterList}{nullSepc}";
    }

    public bool? IsNullable { get; }

    public string Name { get; }

    public IEnumerable<string> Parameters { get; }

    public string Schema { get; }

    private static IEnumerable<string> CollectParameters(DataTypeReference dataTypeRef)
        => dataTypeRef is SqlDataTypeReference sqlRef ?
            sqlRef.Parameters.Select(p => p.Value) :
            new List<string>();

    private string GetName(SchemaObjectName fullyQualifiedName)
        => fullyQualifiedName.BaseIdentifier.Value ?? string.Empty;

    private bool GetNullability(CreateTypeUddtStatement node)
        => node.NullableConstraint is null || node.NullableConstraint.Nullable;

    private string GetSchema(SchemaObjectName fullyQualifiedName)
        => fullyQualifiedName.SchemaIdentifier?.Value ?? string.Empty;
}
