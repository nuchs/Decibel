using Mast.Parsing;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class DbObject : DbFragment
{
    private readonly List<DbObject> referees = new();

    private protected DbObject(TSqlFragment fragment)
        : base(fragment)
    {
    }

    public FullyQualifiedName Identifier { get; protected set; } = new(string.Empty, string.Empty);

    public IEnumerable<DbObject> ReferencedBy => referees;

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

    private protected virtual (IEnumerable<DbObject>, IEnumerable<string>) GetReferents(Database db)
    {
        return (Array.Empty<DbObject>(), Array.Empty<string>());
    }
}
