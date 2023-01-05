using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class Function : DbObject
{
    public Function(CreateFunctionStatement node)
        : base(node)
    {
        Identifier = AssembleIdentifier(node);
        Parameters = CollectParameters(node);
        ReturnType = AssembleReturnType(node);
    }

    public IEnumerable<Parameter> Parameters { get; }

    public string ReturnType { get; }

    internal override IEnumerable<FullyQualifiedName> Constituents
       => base.Constituents.Concat(Parameters.Select(p => FullyQualifiedName.FromName(p.Name)));

    private FullyQualifiedName AssembleIdentifier(CreateFunctionStatement node)
        => FullyQualifiedName.FromSchemaName(GetId(node.Name.SchemaIdentifier), GetId(node.Name.BaseIdentifier));

    private string AssembleReturnType(CreateFunctionStatement node)
        => AssembleFragment(node.ReturnType);

    private List<Parameter> CollectParameters(CreateFunctionStatement node)
        => node.Parameters.Select(p => new Parameter(p)).ToList();
}
