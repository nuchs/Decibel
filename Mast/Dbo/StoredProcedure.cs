using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class StoredProcedure
{
    public StoredProcedure(CreateProcedureStatement node)
    {
        Name = GetName(node);
        Schema = GetSchema(node);
        Content = AssembleFunctionContent(node);
        Parameters = CollectParameters(node);
    }

    public string Content { get; }

    public string Name { get; }

    public List<Parameter> Parameters { get; }

    public string Schema { get; }

    public override string ToString() => Content;

    private static string AssembleFunctionContent(CreateProcedureStatement node)
    {
        var tokenValues = node.ScriptTokenStream.Select(t => t.Text);
        return string.Join(string.Empty, tokenValues);
    }

    private static string GetName(CreateProcedureStatement node)
        => node.ProcedureReference.Name.BaseIdentifier.Value;

    private static string GetSchema(CreateProcedureStatement node)
                => node.ProcedureReference.Name.SchemaIdentifier.Value;

    private List<Parameter> CollectParameters(CreateProcedureStatement node)
        => node.Parameters.Select(p => new Parameter(p)).ToList();
}
