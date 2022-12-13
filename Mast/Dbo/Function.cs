using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class Function : DbObject
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

    private string AssembleReturnType(CreateFunctionStatement node) 
        => AssembleFragment(node.ReturnType);

    private static string GetName(CreateFunctionStatement node)
        => node.Name.BaseIdentifier.Value;

    private static string GetSchema(CreateFunctionStatement node) 
        => node.Name.SchemaIdentifier.Value;

    private List<Parameter> CollectParameters(CreateFunctionStatement node) 
        => node.Parameters.Select(p => new Parameter(p)).ToList();
}
