using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class DbFragment
{
    private protected DbFragment(TSqlFragment fragment)
       => Content = AssembleFragment(fragment);

    public string Content { get; }

    public override string ToString() => Content;

    private protected string AssembleFragment(TSqlFragment fragment)
      => AssembleFragment(
          fragment,
          fragment.FirstTokenIndex,
          fragment.LastTokenIndex + 1);

    private protected string AssembleFragment(TSqlFragment fragment, int start, int end)
    {
        var tokenValues = fragment
            .ScriptTokenStream
            .Take(start..end)
            .Select(t => t.Text);

        return string.Join(string.Empty, tokenValues).Trim();
    }

    private protected string GetId(Identifier? identifier)
        => identifier?.Value ?? string.Empty;
}