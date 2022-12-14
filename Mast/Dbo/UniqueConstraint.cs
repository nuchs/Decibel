using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class UniqueConstraint : DbObject
{
    private readonly List<Column> columns = new();

    public UniqueConstraint(Column column, UniqueConstraintDefinition constraint)
        : this(new[] { column }, constraint)
    {
    }

    public UniqueConstraint(IEnumerable<Column> columns, UniqueConstraintDefinition constraint)
        : base(constraint)
    {
        this.columns.AddRange(columns);
        Clustered = constraint.Clustered ?? false;
    }

    public bool Clustered { get; }

    public IEnumerable<Column> Columns => columns;
}
