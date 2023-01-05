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

    internal override IEnumerable<FullyQualifiedName> Constituents
        => base.Constituents.Concat(Parameters.Select(p => FullyQualifiedName.FromName(p.Name)));

    private FullyQualifiedName AssembleIdentifier(CreateProcedureStatement node)
        => FullyQualifiedName.FromSchemaName(GetId(node.ProcedureReference.Name.SchemaIdentifier), GetId(node.ProcedureReference.Name.BaseIdentifier));

    private List<Parameter> CollectParameters(CreateProcedureStatement node)
        => node.Parameters.Select(p => new Parameter(p)).ToList();
}
