using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class PrimaryKey
{
    private readonly List<Column> columns = new();

    public PrimaryKey(Column column, UniqueConstraintDefinition constraint)
        : this(new[] { column }, constraint)
    {
    }

    public PrimaryKey(IEnumerable<Column> columns, UniqueConstraintDefinition constraint)
    {
        if (!constraint.IsPrimaryKey)
        {
            throw new InvalidOperationException("Cannot create a primary key from a non-primary unqiue constraint");
        }

        this.columns.AddRange(columns);
        Clustered = constraint.Clustered ?? false;
    }

    public bool Clustered { get; }

    public IEnumerable<Column> Columns => columns;
}
