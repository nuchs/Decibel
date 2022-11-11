using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mast.Dbo;
public class Index
{
    public List<Column> Columns = new();
    public string Name = string.Empty;

    public Index(List<Column> tableColumns, IndexDefinition index)
    {
        Name = index.Name.Value;

        foreach (var col in index.Columns)
        {
            var idParts = col.Column.MultiPartIdentifier.Identifiers.Select(i => i.Value);
            var id = string.Join('.', idParts);

            var c = tableColumns.Where(t => t.name == id);

            if (c.Count()!= 1)
            {
                throw new InvalidOperationException("Wrong number of columns");
            }

            Columns.Add(c.First());
        }
    }
}
