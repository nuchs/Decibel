using Mast.Parsing;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public abstract class DbObject : DbFragment
{
    private readonly HashSet<DbObject> referees = new();
    private readonly TSqlFragment fragment;

    protected DbObject(TSqlFragment fragment)
        : base(fragment)
        => this.fragment = fragment;

    public FullyQualifiedName Identifier { get; protected set; } = FullyQualifiedName.None;

    public IEnumerable<DbObject> ReferencedBy => referees;

    internal void CrossReference(Database db)
    {
        HashSet<TSqlTokenType> idTokens = new() { TSqlTokenType.Database, TSqlTokenType.Schema, TSqlTokenType.Dot, TSqlTokenType.Identifier, TSqlTokenType.QuotedIdentifier };
        List<string> idParts = new();
        List<FullyQualifiedName> unresolved = new();

        foreach (var token in fragment.ScriptTokenStream.Skip(fragment.FirstTokenIndex).Take(fragment.FragmentLength))
        {
            if (idTokens.Contains(token.TokenType))
            {
                if (token.TokenType != TSqlTokenType.Dot)
                {
                    idParts.Add(token.Text);
                }
            }
            else if (idParts.Count > 0)
            {
                var candidate = idParts.Count > 1 ?
                    FullyQualifiedName.FromSchemaName(idParts[0], idParts[1]) :
                    FullyQualifiedName.FromName(idParts[0]);

                if (candidate.Schema != string.Empty)
                {
                    var schemaName = FullyQualifiedName.FromSchema(candidate.Schema);

                    if (schemaName != Identifier)
                    {
                        if (db.NameMap.TryGetValue(schemaName, out var schema))
                        {

                        schema.referees.Add(this);
                        }
                        else
                        {
                            unresolved.Add(schemaName);
                        }
                    }
                }


                if (candidate != Identifier )
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

                idParts = new();
            }
        }

        db.AddUnresolvedRefs(this, unresolved);
    }

    private protected (IEnumerable<DbObject>, IEnumerable<FullyQualifiedName>) CorralateRefs(IEnumerable<DbObject> candidates, FullyQualifiedName target)
        => CorralateRefs(candidates, new[] { target });

    private protected (IEnumerable<DbObject>, IEnumerable<FullyQualifiedName>) CorralateRefs(IEnumerable<DbObject> candidates, IEnumerable<FullyQualifiedName> targets)
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

    private protected abstract (IEnumerable<DbObject>, IEnumerable<FullyQualifiedName>) GetReferents(Database db);
}
