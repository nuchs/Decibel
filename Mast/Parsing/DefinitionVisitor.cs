using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Parsing;

internal sealed class DefinitionVisitor : TSqlFragmentVisitor
{
    private readonly Database db;

    public DefinitionVisitor(Database db) => this.db = db;

    public override void Visit(CreateTableStatement node) => Visit(db.TableList, new(node), node);

    public override void Visit(CreateTypeTableStatement node) => Visit(db.TableTypeList, new(node), node);

    public override void Visit(CreateTypeUddtStatement node) => Visit(db.ScalarTypeList, new(node), node);

    public override void Visit(CreateProcedureStatement node) => Visit(db.ProcedureList, new(node), node);

    public override void Visit(CreateFunctionStatement node) => Visit(db.FunctionList, new(node), node);

    public override void Visit(CreateSchemaStatement node) => Visit(db.SchemaList, new(node), node);

    public override void Visit(CreateTriggerStatement node) => Visit(db.TriggerList, new(node), node);

    public override void Visit(CreateViewStatement node) => Visit(db.ViewList, new(node), node);

    public override void Visit(CreateUserStatement node) => Visit(db.UserList, new(node), node);

    private void Visit<T, U>(List<U> mastList, U mastItem, T node) where T : TSqlFragment
    {
        mastList.Add(mastItem);
        base.Visit(node);
    }
}
