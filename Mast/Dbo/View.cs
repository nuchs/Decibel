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

    public IEnumerable<FullyQualifiedName> BaseTables { get; }

    public bool Check { get; }

    public IEnumerable<string> Columns { get; }

    public bool SchemaBinding { get; }

    internal override IEnumerable<FullyQualifiedName> Constituents
       => base.Constituents.Concat(Columns.Select(FullyQualifiedName.FromName));

    private FullyQualifiedName AssembleIdentifier(CreateViewStatement node)
        => FullyQualifiedName.FromSchemaName(GetId(node.SchemaObjectName.SchemaIdentifier), GetId(node.SchemaObjectName.BaseIdentifier));

    private IEnumerable<string> CollectColumns(CreateViewStatement view)
            => view.Columns.Select(c => c.Value);

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

    private bool GetCheckOption(CreateViewStatement view)
        => view.WithCheckOption;

    private bool GetSchemaBinding(CreateViewStatement view)
        => view.ViewOptions.Any(o => o.OptionKind == ViewOptionKind.SchemaBinding);
}
