using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class DbObject
{
    protected DbObject(TSqlFragment fragment)
    {
        Content = AssembleFragment(fragment);
    }

    public string Content { get; }

    public string Name { get; protected set; } = string.Empty;

    public override string ToString() => Content;

    protected string AssembleFragment(TSqlFragment fragment) =>
        AssembleFragment(fragment, fragment.FirstTokenIndex, fragment.LastTokenIndex + 1);

    protected string AssembleFragment(TSqlFragment fragment, int start, int end)
    {
        var tokenValues = fragment
            .ScriptTokenStream
            .Take(start..end)
            .Select(t => t.Text);

        return string.Join(string.Empty, tokenValues).Trim();
    }

    protected string GetId(Identifier identifier) 
        => identifier?.Value ?? string.Empty;
}
