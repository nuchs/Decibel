using Mast.Parsing;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class DbObject : DbFragment
{
    private static readonly IReadOnlyCollection<FullyQualifiedName> noIgnore = new HashSet<FullyQualifiedName>();
    private readonly TSqlFragment fragment;

    private protected DbObject(TSqlFragment fragment)
        : base(fragment)
    {
        this.fragment = fragment;
    }

    public FullyQualifiedName Identifier { get; protected set; } = FullyQualifiedName.None;

    public IEnumerable<DbObject> ReferencedBy => Referees;

    internal virtual IEnumerable<FullyQualifiedName> Constituents => new[] { Identifier };

    internal HashSet<DbObject> Referees { get; } = new();

    internal void CrossReference(Database db)
    {
        FqnBuilder idParts = new();
        ReferenceVisitor referrer = new(db, this);
        fragment.Accept(referrer);

        foreach (var token in fragment.FragmentStream())
        {
            idParts.AddToken(token);
            idParts = ProcessIdStream(db, idParts, referrer.Ignore);
        }

        // Handle case where last token is final part of an identifier
        idParts.Build();
        ProcessIdStream(db, idParts, referrer.Ignore);

        ResolveObjectSchema(db);
    }

    private void ResolveObjectSchema(Database db)
    {
        var candidate = FullyQualifiedName.FromDbSchema(Identifier.Db, Identifier.Schema);

        if (candidate != Identifier)
        {
            db.ResolveReference(this, candidate); 
        }
    }

    private FqnBuilder ProcessIdStream(Database db, FqnBuilder idParts, IReadOnlyCollection<FullyQualifiedName> ignore)
    {
        if (idParts.IsReady)
        {
            var candidate = idParts.Id;

            if (!(ignore.Contains(candidate) || Constituents.Contains(candidate)))
            {
                if (HasSchema(candidate))
                {
                    db.ResolveReference(this, FullyQualifiedName.FromDbSchema(candidate.Db, candidate.Schema));
                }

                db.ResolveReference(this, candidate);
            }

            idParts = new();
        }

        return idParts;
    }

    private static bool HasSchema(FullyQualifiedName candidate) => !string.IsNullOrWhiteSpace(candidate.Schema);
}
