using Microsoft.SqlServer.TransactSql.ScriptDom;

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

            var c = tableColumns.Where(t => t.Name == id);

            if (c.Count() != 1)
            {
                throw new InvalidOperationException("Wrong number of columns");
            }

            Columns.Add(c.First());
        }
    }
}
