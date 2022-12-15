using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class UniqueConstraint : DbObject
{
    public UniqueConstraint(Column column, UniqueConstraintDefinition constraint)
        : this(new[] { column }, constraint)
    {
    }

    public UniqueConstraint(IEnumerable<Column> columns, UniqueConstraintDefinition constraint)
        : base(constraint)
    {
        Columns = columns;
        Clustered = constraint.Clustered ?? false;
    }

    public bool Clustered { get; }

    public IEnumerable<Column> Columns  { get; }
}
