using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class DataType
{
    public DataType(DataTypeReference dataTypeRef)
    {
        Name = dataTypeRef.Name.BaseIdentifier.Value;
        Schema = dataTypeRef.Name.SchemaIdentifier?.Value ?? string.Empty;

        Parameter = dataTypeRef is SqlDataTypeReference sqlRef ? 
            sqlRef.Parameters.Select(p => p.Value) : 
            new List<string>();
    }

    public string Name { get; set; }

    public IEnumerable<string> Parameter { get; set; }

    public string Schema { get; set; }
}
