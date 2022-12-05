using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

internal class Visitor : TSqlFragmentVisitor
{
    private readonly Database db;

    public Visitor(Database db)
    {
        this.db = db;
    }

    public override void Visit(CreateTableStatement node)
    {
        Table table = new(node);

        db.Tables.Add(table);

        base.Visit(node);
    }

    public override void Visit(CreateTypeTableStatement node)
    {
        TableType type = new(node);

        db.Types.Add(type);

        base.Visit(node);
    }

    public override void Visit(CreateProcedureStatement node)
    {
        StoredProcedure proc = new(node);

        db.Procedures.Add(proc);

        base.Visit(node);
    }

    public override void Visit(CreateFunctionStatement node)
    {
        Function function = new(node);

        db.Functions.Add(function);

        base.Visit(node);
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
