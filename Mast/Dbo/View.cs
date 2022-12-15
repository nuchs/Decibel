using Mast.Parsing;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class View : DbObject
{
    public View(CreateViewStatement view)
        : base(view)
    {
        Name = GetName(view);
        Schema = GetSchema(view);
        Columns = CollectColumns(view);
        SchemaBinding = GetSchemaBinding(view);
        Check = GetCheckOption(view);
    }

    public bool Check { get; }

    public IEnumerable<string> Columns { get; }

    public string Schema { get; }

    public bool SchemaBinding { get; }

    internal override void CrossReference(Database db) => throw new NotImplementedException();
    private IEnumerable<string> CollectColumns(CreateViewStatement view)
        => view.Columns.Select(c => c.Value);

    private bool GetCheckOption(CreateViewStatement view)
        => view.WithCheckOption;

    private string GetName(CreateViewStatement view)
        => GetId(view.SchemaObjectName.BaseIdentifier);

    private string GetSchema(CreateViewStatement view)
        => GetId(view.SchemaObjectName.SchemaIdentifier);

    private bool GetSchemaBinding(CreateViewStatement view)
        => view.ViewOptions.Any(o => o.OptionKind == ViewOptionKind.SchemaBinding);
}
