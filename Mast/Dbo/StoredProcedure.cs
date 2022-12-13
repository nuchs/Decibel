using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class StoredProcedure :DbObject
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

    private static string GetName(CreateProcedureStatement node)
        => node.ProcedureReference.Name.BaseIdentifier.Value;

    private static string GetSchema(CreateProcedureStatement node)
                => node.ProcedureReference.Name.SchemaIdentifier.Value;

    private List<Parameter> CollectParameters(CreateProcedureStatement node)
        => node.Parameters.Select(p => new Parameter(p)).ToList();
}
