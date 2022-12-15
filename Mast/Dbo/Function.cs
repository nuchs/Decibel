using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class Function : DbObject
{
    public Function(CreateFunctionStatement node)
        : base(node)
    {
        Name = GetName(node);
        Schema = GetSchema(node);
        Parameters = CollectParameters(node);
        ReturnType = AssembleReturnType(node);
    }

    public IEnumerable<Parameter> Parameters { get; }

    public string ReturnType { get; }

    public string Schema { get; }

    private string GetName(CreateFunctionStatement node)
        => GetId(node.Name.BaseIdentifier);

    private string GetSchema(CreateFunctionStatement node)
        => GetId(node.Name.SchemaIdentifier);

    private string AssembleReturnType(CreateFunctionStatement node)
        => AssembleFragment(node.ReturnType);

    private List<Parameter> CollectParameters(CreateFunctionStatement node)
        => node.Parameters.Select(p => new Parameter(p)).ToList();
}
