using Mast.Parsing;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class DbObject : DbFragment
{
    private readonly TSqlFragment fragment;
    private readonly HashSet<DbObject> referees = new();

    private protected DbObject(TSqlFragment fragment)
        : base(fragment)
        => this.fragment = fragment;

    public FullyQualifiedName Identifier { get; protected set; } = FullyQualifiedName.None;

    public IEnumerable<DbObject> ReferencedBy => referees;

    internal void CrossReference(Database db)
    {
        FqnBuilder idParts = new();
        List<FullyQualifiedName> unresolved = new();

        foreach (var token in fragment.ScriptTokenStream.Skip(fragment.FirstTokenIndex).Take(fragment.FragmentLength))
        {
            idParts.AddToken(token);

            if (idParts.IsReady)
            {
                var candidate = idParts.Id;

                if (!string.IsNullOrWhiteSpace(candidate.Schema))
                {
                    ResolveReference(db, FullyQualifiedName.FromSchema(candidate.Schema), unresolved);
                }

                ResolveReference(db, candidate, unresolved);

                idParts = new();
            }
        }

        db.AddUnresolvedRefs(this, unresolved);
    }

    private void ResolveReference(Database db, FullyQualifiedName candidate, List<FullyQualifiedName> unresolved)
    {
        if (candidate != Identifier)
        {
            if (db.NameMap.TryGetValue(candidate, out var referent))
            {
                referent.referees.Add(this);
            }
            else
            {
                unresolved.Add(candidate);
            }
        }
    }
}
