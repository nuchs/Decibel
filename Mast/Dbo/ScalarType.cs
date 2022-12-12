using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class ScalarType
{
    public ScalarType(DataTypeReference dataTypeRef)
    {
        Name = GetName(dataTypeRef.Name);
        Schema = GetSchema(dataTypeRef.Name);
        Parameters = CollectParameters(dataTypeRef);
        Content = AssembleTypeContent(dataTypeRef.ScriptTokenStream, dataTypeRef.FirstTokenIndex, dataTypeRef.LastTokenIndex);
    }

    public ScalarType(CreateTypeUddtStatement node)
    {
        Name = GetName(node.Name);
        Schema = GetSchema(node.Name);
        IsNullable = GetNullability(node);
        Parameters = CollectParameters(node.DataType);
        Content = AssembleTypeContent(node.ScriptTokenStream, node.FirstTokenIndex, node.LastTokenIndex);
    }

    public string Content { get; }

    public bool? IsNullable { get; }

    public string Name { get; }

    public IEnumerable<string> Parameters { get; }

    public string Schema { get; }

    public override string ToString() => Content;

    private static string AssembleTypeContent(IList<TSqlParserToken> tokenStream, int first, int last)
    {
        var tokenValues = tokenStream.Take(first .. (last+1)).Select(t => t.Text);
        return string.Join(string.Empty, tokenValues);
    }

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
