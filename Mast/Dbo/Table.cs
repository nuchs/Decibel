using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class Table : DbObject
{
    public List<Column> Columns = new();
    public List<object> Constraints = new();
    public List<object> ForeignKeys = new();
    public PrimaryKey? PrimaryKey { get; }
    public List<object> ReferencedBy = new();
    public IEnumerable<Index> Indices { get; }
    public string Schema;

    public Table(CreateTableStatement node)
        : base(node)
    {
        Schema = node.SchemaObjectName.SchemaIdentifier.Value;
        PrimaryKey = GetPrimary(node);
        var identifiers = node.SchemaObjectName.Identifiers.Skip(1).Select(id => id.Value);
        Name = string.Join('.', identifiers);

        foreach (var colDef in node.Definition.ColumnDefinitions)
        {
            Column column = new Column(colDef);
            Columns.Add(column);
        }

        Indices = CollectIndices(node);
    }

    private IEnumerable<Index> CollectIndices(CreateTableStatement node)
        => node.Definition.Indexes.Select(i => new Index(Columns, i));

    private PrimaryKey? GetPrimary(CreateTableStatement node)
    {
        var primaryCol = Columns.Select(c => c.PrimaryKey).FirstOrDefault(p => p is not null);

        if (primaryCol is not null)
        {
            return primaryCol;
        }

        var compoundPrimary = node.Definition.TableConstraints.OfType<UniqueConstraintDefinition>().FirstOrDefault(uq => uq.IsPrimaryKey);

        if (compoundPrimary is not null)
        {
            return new(Columns, compoundPrimary);
        }

        return null;
    }
}
