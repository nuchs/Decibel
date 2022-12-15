using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class CheckConstraint : DbFragment
{
    public CheckConstraint(CheckConstraintDefinition constraint)
        : base(constraint)
    {
        Name = GetName(constraint);
    }

    public string Name { get; }

    private string GetName(CheckConstraintDefinition constraint)
        => GetId(constraint.ConstraintIdentifier);
}
