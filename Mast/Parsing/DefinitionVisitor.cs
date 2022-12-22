using Mast.Dbo;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Parsing;

internal sealed class DefinitionVisitor : TSqlFragmentVisitor
{
    private readonly Database db;

    public DefinitionVisitor(Database db) => this.db = db;

    public override void Visit(CreateTableStatement node) => Visit(new Table(node), node);

    public override void Visit(CreateTypeTableStatement node) => Visit(new TableType(node), node);

    public override void Visit(CreateTypeUddtStatement node) => Visit(new ScalarType(node), node);

    public override void Visit(CreateProcedureStatement node) => Visit(new StoredProcedure(node), node);

    public override void Visit(CreateFunctionStatement node) => Visit(new Function(node), node);

    public override void Visit(CreateSchemaStatement node) => Visit(new Schema(node), node);

    public override void Visit(CreateTriggerStatement node) => Visit(new Trigger(node), node);

    public override void Visit(CreateViewStatement node) => Visit(new View(node), node);

    public override void Visit(CreateUserStatement node) => Visit(new User(node), node);

    private void Visit<T>(DbObject mastItem, T node)
        where T : TSqlFragment
    {
        db.AddObject(mastItem);
        base.Visit(node);
    }
}
