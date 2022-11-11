using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mast.Dbo;
public class Table
{
    public string Content;
    public List<object> ReferencedBy = new();
    public List<object> References = new();
    public string Name;
    public string Schema;
    public List<Column> Columns = new();
    public List<object> ForeignKeys = new();
    public List<object> Constraints = new();
    public object PrimaryKey;

    public Table(CreateTableStatement node)
    {
        var tokenValues = node.ScriptTokenStream.Select(t => t.Text);
        Content = string.Join(string.Empty, tokenValues);

        Schema = node.SchemaObjectName.SchemaIdentifier.Value;

        var identifiers = node.SchemaObjectName.Identifiers.Skip(1).Select(id => id.Value);
        Name = string.Join('.', identifiers);

        foreach (var colDef in node.Definition.ColumnDefinitions)
        {
            Column column = new Column(colDef);
            Columns.Add(column);
        }
    }
}
