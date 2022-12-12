using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class DbObject
{
    protected DbObject(TSqlFragment fragment)
    {
        Content = AssembleFragment(fragment);
    }

    public string Name { get; protected set; }

    public string Content { get; }

    public override string ToString() => Content;

    protected string AssembleFragment(TSqlFragment fragment)
    {
        var tokenValues = fragment
         .ScriptTokenStream
         .Take(fragment.FirstTokenIndex..(fragment.LastTokenIndex + 1))
         .Select(t => t.Text);

        return string.Join(string.Empty, tokenValues);
    }
}
