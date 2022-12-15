using Mast.Parsing;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public abstract class DbObject
{
    private readonly List<DbObject> referees = new();

    private protected DbObject(TSqlFragment fragment)
        => Content = AssembleFragment(fragment);

    public string Content { get; }

    public string Name { get; protected set; } = string.Empty;

    public IEnumerable<DbObject> ReferencedBy => referees;

    public override string ToString() => Content;

    internal abstract void CrossReference(Database db);

    private protected void AddReferee(DbObject referee) => referees.Add(referee);

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
