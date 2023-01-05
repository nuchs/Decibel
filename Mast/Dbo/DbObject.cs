using Mast.Parsing;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class DbObject : DbFragment
{
    private static readonly Aliases NoAlias = new(Array.Empty<TSqlParserToken>());
    private readonly IEnumerable<TSqlParserToken> tokenStream;
    private readonly Aliases aliases;

    protected DbObject(TSqlFragment fragment)
        : base(fragment)
    {
        tokenStream = fragment.ScriptTokenStream
            .Skip(fragment.FirstTokenIndex)
            .Take(fragment.LastTokenIndex - fragment.FirstTokenIndex + 1);
        aliases = new(tokenStream);
    }

    public FullyQualifiedName Identifier { get; protected set; } = FullyQualifiedName.None;

    public IEnumerable<DbObject> ReferencedBy => Referees;

    internal virtual IEnumerable<FullyQualifiedName> Constituents => new[] { Identifier };
    internal HashSet<DbObject> Referees { get; } = new();

    internal void CrossReference(Database db)
    {
        FqnBuilder idParts = new();

        foreach (var token in tokenStream)
        {
            idParts.AddToken(token);
            idParts = ProcessIdStream(db, idParts);
        }

        // Handle case where last token is final part of an identifier
        idParts.Build();
        ProcessIdStream(db, idParts);
    }

    private FqnBuilder ProcessIdStream(Database db, FqnBuilder idParts)
    {
        if (idParts.IsReady)
        {
            var candidate = idParts.Id;

            if (!string.IsNullOrWhiteSpace(candidate.Schema))
            {
                db.ResolveReference(this, FullyQualifiedName.FromSchema(candidate.Schema), NoAlias);
            }

            db.ResolveReference(this, candidate, aliases);

            idParts = new();
        }

        return idParts;
    }
}
