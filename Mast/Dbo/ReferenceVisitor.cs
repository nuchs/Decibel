using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

internal class ReferenceVisitor : TSqlFragmentVisitor
{
    private readonly Database db;

    public ReferenceVisitor(Database db)
    {
        this.db = db;
    }

    public override void Visit(FunctionCall node)
    {
        base.Visit(node);
    }

    public override void Visit(ExecuteStatement node)
    {
        base.Visit(node);
    }
}
