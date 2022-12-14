using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class CheckConstraint : DbObject
{
    public CheckConstraint(CheckConstraintDefinition constaint)
        : base(constaint)
    {
    }
}
