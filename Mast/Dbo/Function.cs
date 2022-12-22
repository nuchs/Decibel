using Mast.Parsing;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Linq;

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

    private FullyQualifiedName AssembleIdentifier(CreateFunctionStatement node)
        => FullyQualifiedName.FromSchemaName(GetId(node.Name.SchemaIdentifier), GetId(node.Name.BaseIdentifier));

    private string AssembleReturnType(CreateFunctionStatement node)
        => AssembleFragment(node.ReturnType);

    private List<Parameter> CollectParameters(CreateFunctionStatement node)
        => node.Parameters.Select(p => new Parameter(p)).ToList();
}
