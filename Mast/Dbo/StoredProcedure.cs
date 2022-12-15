using Mast.Parsing;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class StoredProcedure : DbObject
{
    public StoredProcedure(CreateProcedureStatement node)
        : base(node)
    {
        Name = GetName(node);
        Schema = GetSchema(node);
        Parameters = CollectParameters(node);
    }

    public List<Parameter> Parameters { get; }

    public string Schema { get; }

    private string GetName(CreateProcedureStatement node)
        => GetId(node.ProcedureReference.Name.BaseIdentifier);

    private string GetSchema(CreateProcedureStatement node)
        => GetId(node.ProcedureReference.Name.SchemaIdentifier);

    private List<Parameter> CollectParameters(CreateProcedureStatement node)
        => node.Parameters.Select(p => new Parameter(p)).ToList();
    internal override void CrossReference(Database db) => throw new NotImplementedException();
}
