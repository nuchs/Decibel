using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class DbObject
{
    private readonly List<DbObject> referees = new();

    protected DbObject(TSqlFragment fragment) 
        => Content = AssembleFragment(fragment);

    public string Content { get; }

    public string Name { get; protected set; } = string.Empty;

    public IEnumerable<DbObject> ReferencedBy => referees;

    public override string ToString() => Content;

    protected void AddReferee(DbObject referee) => referees.Add(referee);

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
