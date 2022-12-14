using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class Table : DbObject
{
    public List<Column> Columns = new();
    public List<object> Constraints = new();
    public List<object> ForeignKeys = new();
    public object PrimaryKey;
    public List<object> ReferencedBy = new();
    public string Schema;

    public Table(CreateTableStatement node)
        : base(node)
    {
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
