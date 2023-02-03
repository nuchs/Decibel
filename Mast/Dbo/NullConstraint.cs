using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class NullConstraint : DbFragment
{
    public NullConstraint(NullableConstraintDefinition constraint)
        : base(constraint)
    {
        Name = GetName(constraint);
        IsNullable = constraint.Nullable;
    }

    public string Name { get; }

    public bool IsNullable { get; }

    private string GetName(NullableConstraintDefinition constraint)
        => GetId(constraint.ConstraintIdentifier);
}
