using Mast.Parsing;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class CheckConstraint : DbObject
{
    public CheckConstraint(CheckConstraintDefinition constaint)
        : base(constaint)
    {
    }

    internal override void CrossReference(Database db) => throw new NotImplementedException();
}
