using Mast.Dbo;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Parsing;

internal class DefinitionVisitor : TSqlFragmentVisitor
{
    private readonly Database db;

    public DefinitionVisitor(Database db)
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

        db.TableTypes.Add(type);

        base.Visit(node);
    }


    public override void Visit(CreateTypeUddtStatement node)
    {
        ScalarType type = new(node);

        db.ScalarTypes.Add(type);

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

    public override void Visit(CreateSchemaStatement node)
    {
        base.Visit(node);
    }

    public override void Visit(CreateTriggerStatement node)
    {
        base.Visit(node);
    }

    public override void Visit(CreateViewStatement node)
    {
        base.Visit(node);
    }

    public override void Visit(CreateUserStatement node)
    {
        // Are grants separate?
        base.Visit(node);
    }
}
