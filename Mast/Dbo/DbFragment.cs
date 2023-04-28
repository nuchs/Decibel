using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class DbFragment
{
    protected DbFragment(TSqlFragment fragment)
       => Content = AssembleFragment(fragment);

    public string Content { get; }

    public static bool operator !=(DbFragment? left, DbFragment? right)
        => !(left == right);

    public static bool operator ==(DbFragment? left, DbFragment? right)
        => (left, right) switch
        {
            (null, null) => true,
            (null, _) => false,
            (_, null) => false,
            _ => left.Equals(right),
        };

    public override bool Equals(object? obj)
        => obj is DbFragment other && new CaseInsensitiveString(Content) == new CaseInsensitiveString(other.Content);

    public override int GetHashCode() => Content.GetHashCode();

    public override string ToString() => Content;

    protected string AssembleFragment(TSqlFragment fragment)
      => AssembleFragment(
          fragment,
          fragment.FirstTokenIndex,
          fragment.LastTokenIndex + 1);

    protected string AssembleFragment(TSqlFragment fragment, int start, int end)
    {
        var tokenValues = fragment
            .ScriptTokenStream
            .Take(start..end)
            .Select(t => t.Text);

        return string.Join(string.Empty, tokenValues).Trim();
    }

    protected string GetId(Identifier? identifier)
        => identifier?.Value ?? string.Empty;
}