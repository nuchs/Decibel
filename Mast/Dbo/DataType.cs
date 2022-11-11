using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mast.Dbo;
public class DataType
{
    public string Name;
    public string Schema;
    public IEnumerable<string> Parameter;

    public DataType(DataTypeReference dataTypeRef)
    {
        Name = dataTypeRef.Name.BaseIdentifier.Value;
        Schema = dataTypeRef.Name.SchemaIdentifier?.Value ?? String.Empty;

        if(dataTypeRef is SqlDataTypeReference sqlRef)
        {
            Parameter = sqlRef.Parameters.Select(p => p.Value);
        }
    }
}
