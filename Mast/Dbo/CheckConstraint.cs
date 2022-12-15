using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class CheckConstraint : DbObject
{
    public CheckConstraint(CheckConstraintDefinition constaint)
        : base(constaint)
    {
    }

    // TODO - where's the id
}
