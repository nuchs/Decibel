using Mast.Parsing;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class DbObject : DbFragment
{
    private readonly HashSet<DbObject> referees = new();

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

    protected (IEnumerable<DbObject>, IEnumerable<FullyQualifiedName>) CorralateRefs(IEnumerable<DbObject> candidates, FullyQualifiedName target)
        => CorralateRefs(candidates, new[] { target });

    protected (IEnumerable<DbObject>, IEnumerable<FullyQualifiedName>) CorralateRefs(IEnumerable<DbObject> candidates, IEnumerable<FullyQualifiedName> targets)
    {
        List<FullyQualifiedName> unresolved = new();
        List<DbObject> referents = new();

        foreach (var target in targets)
        {
            var referent = candidates.Where(c => c.Identifier == target);

            if (referent.Any())
            {
                referents.AddRange(referent);
            }
            else
            {
                unresolved.Add(target);
            }
        }

        return (referents, unresolved);
    }

    // TODO make abstract
    private protected virtual (IEnumerable<DbObject>, IEnumerable<FullyQualifiedName>) GetReferents(Database db)
    {
        return (Array.Empty<DbObject>(), Array.Empty<FullyQualifiedName>());
    }
}
