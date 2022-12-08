using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class Function
{
    public Function(CreateFunctionStatement node)
    {
        Schema = GetFunctionSchema(node);
        Name = GetFunctionName(node);
        Content = AssembleFunctionContent(node);
        ReturnType = AssembleReturnType(node);
        Parameters = CollectParameters(node);
    }

    public string Content { get; }

    public string Name { get; }

    public IEnumerable<Parameter> Parameters { get; }

    public string ReturnType { get; }

    public string Schema { get; }

    public override string ToString() => Content;

    private static string AssembleFunctionContent(CreateFunctionStatement node)
    {
        var tokenValues = node.ScriptTokenStream.Select(t => t.Text);
        return string.Join(string.Empty, tokenValues);
    }

    private static string AssembleReturnType(CreateFunctionStatement node)
    {
        var rtnTokens = node.ScriptTokenStream
            .Take(node.ReturnType.FirstTokenIndex..(node.ReturnType.LastTokenIndex + 1))
            .Select(f => f.Text);

        return string.Join("", rtnTokens).Trim();
    }

    private static string GetFunctionName(CreateFunctionStatement node)
        => node.Name.BaseIdentifier.Value;

    private static string GetFunctionSchema(CreateFunctionStatement node) 
        => node.Name.SchemaIdentifier.Value;

    private List<Parameter> CollectParameters(CreateFunctionStatement node) 
        => node.Parameters.Select(p => new Parameter(p)).ToList();
}
