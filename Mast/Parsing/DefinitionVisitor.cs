using Mast.Dbo;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Parsing;

internal sealed class DefinitionVisitor : TSqlFragmentVisitor
{
    private readonly Database db;

    public DefinitionVisitor(Database db) => this.db = db;

    public override void Visit(CreateTableStatement node) => db.AddObject(new Table(node));

    public override void Visit(CreateTypeTableStatement node) => db.AddObject(new TableType(node));

    public override void Visit(CreateTypeUddtStatement node) => db.AddObject(new ScalarType(node));

    public override void Visit(CreateProcedureStatement node) => db.AddObject(new StoredProcedure(node));

    public override void Visit(CreateFunctionStatement node) => db.AddObject(new Function(node));

    public override void Visit(CreateSchemaStatement node) => db.AddObject(new Schema(node));

    public override void Visit(CreateTriggerStatement node) => db.AddObject(new Trigger(node));

    public override void Visit(CreateViewStatement node) => db.AddObject(new View(node));

    public override void Visit(CreateUserStatement node) => db.AddObject(new User(node));
}
