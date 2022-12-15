using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class UniqueConstraint : DbFragment
{
    public UniqueConstraint(Column column, UniqueConstraintDefinition constraint)
        : this(new[] { column }, constraint)
    {
    }

    public UniqueConstraint(IEnumerable<Column> columns, UniqueConstraintDefinition constraint)
        : base(constraint)
    {
        Name = GetName(constraint);
        Columns = columns;
        Clustered = constraint.Clustered ?? false;
    }

    public bool Clustered { get; }

    public IEnumerable<Column> Columns { get; }

    public string Name { get; }

    private string GetName(UniqueConstraintDefinition constraint)
        => GetId(constraint.ConstraintIdentifier);
}
