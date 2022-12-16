using Mast.Parsing;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class View : DbObject
{
    public View(CreateViewStatement view)
        : base(view)
    {
        Identifier = AssembleIdentifier(view);
        Columns = CollectColumns(view);
        SchemaBinding = GetSchemaBinding(view);
        Check = GetCheckOption(view);
        BaseTables = CollectTables(view);
    }

    private IEnumerable<FullyQualifiedName> CollectTables(CreateViewStatement view)
    {
        IEnumerable<FullyQualifiedName>? result = null;

        if (view.SelectStatement.QueryExpression is QuerySpecification query)
        {
            result = query.FromClause?
                .TableReferences?
                .OfType<NamedTableReference>()
                .Select(t => new FullyQualifiedName(
                    GetId(t.SchemaObject.DatabaseIdentifier), 
                    GetId(t.SchemaObject.SchemaIdentifier), 
                    GetId(t.SchemaObject.BaseIdentifier)));
        }
        
        return result ?? Array.Empty<FullyQualifiedName>();
    }

    public bool Check { get; }

    public IEnumerable<string> Columns { get; }

    public bool SchemaBinding { get; }

    public IEnumerable<FullyQualifiedName> BaseTables { get; }

    private protected override (IEnumerable<DbObject>, IEnumerable<FullyQualifiedName>) GetReferents(Database db)
    {
        var (schemaHits, schmeaMisses) = CorralateRefs(db.Schemas, FullyQualifiedName.FromSchema(Identifier.Schema));
        var (tableHits, tableMisses) = CorralateRefs(db.Tables, BaseTables);

        return (schemaHits.Concat(tableHits), schmeaMisses.Concat(tableMisses));
    }

    private FullyQualifiedName AssembleIdentifier(CreateViewStatement node)
        => FullyQualifiedName.FromSchemaName(GetId(node.SchemaObjectName.SchemaIdentifier), GetId(node.SchemaObjectName.BaseIdentifier));

    private IEnumerable<string> CollectColumns(CreateViewStatement view)
            => view.Columns.Select(c => c.Value);

    private bool GetCheckOption(CreateViewStatement view)
        => view.WithCheckOption;

    private bool GetSchemaBinding(CreateViewStatement view)
        => view.ViewOptions.Any(o => o.OptionKind == ViewOptionKind.SchemaBinding);
}
