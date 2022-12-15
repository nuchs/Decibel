using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class StoredProcedure : DbObject
{
    public StoredProcedure(CreateProcedureStatement node)
        : base(node)
    {
        Identifier = AssembleIdentifier(node);
        Parameters = CollectParameters(node);
    }

    public List<Parameter> Parameters { get; }

    private FullyQualifiedName AssembleIdentifier(CreateProcedureStatement node)
        => new(GetId(node.ProcedureReference.Name.SchemaIdentifier), GetId(node.ProcedureReference.Name.BaseIdentifier));

    private List<Parameter> CollectParameters(CreateProcedureStatement node)
        => node.Parameters.Select(p => new Parameter(p)).ToList();
}
