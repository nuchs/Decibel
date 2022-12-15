using Mast.Parsing;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class DbObject
{
    private readonly List<DbObject> referees = new();

    private protected DbObject(TSqlFragment fragment)
        => Content = AssembleFragment(fragment);

    public string Content { get; }

    public string Name { get; protected set; } = string.Empty;

    public IEnumerable<DbObject> ReferencedBy => referees;

    public override string ToString() => Content;

    internal void CrossReference(Database db)
    {
        var (referents, unresolvedRefs) = GetReferents(db);

        foreach (var dbo in referents)
        {
            dbo.referees.Add(this);
        }

        foreach (var reference in unresolvedRefs)
        {
            db.UnresolvedReferencesList.Add(new(this, reference));
        }
    }

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

    private protected virtual (IEnumerable<DbObject>, IEnumerable<string>) GetReferents(Database db)
    {
        return (Array.Empty<DbObject>(), Array.Empty<string>());
    }
}
