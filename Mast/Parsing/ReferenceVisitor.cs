using Mast.Dbo;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Parsing;

internal class ReferenceVisitor : TSqlFragmentVisitor
{
    private readonly Database db;
    private readonly DbObject root;
    private readonly HashSet<FullyQualifiedName> ignore = new();

    internal ReferenceVisitor(Database db, DbObject root)
    {
        this.db = db;
        this.root = root;
    }

    internal IReadOnlyCollection<FullyQualifiedName> Ignore => ignore;

    public override void Visit(InsertStatement node)
    {
        FqnBuilder idParts = new();
        var columnList = false;

        foreach (var token in node.FragmentStream())
        {
            if (token.TokenType == TSqlTokenType.Values)
            {
                return;
            }

            if (token.TokenType == TSqlTokenType.LeftParenthesis)
            {
                columnList = true;
            }

            if (columnList)
            {
                idParts.AddToken(token);

                if (idParts.IsReady)
                {
                    ignore.Add(idParts.Id);
                    idParts = new();
                }
            }
        }
    }

    public override void Visit(UpdateStatement node) => ProcessNonExplicitIdentifiers(node);

    public override void Visit(DeleteStatement node) => ProcessNonExplicitIdentifiers(node);

    public override void Visit(SelectStatement node) => ProcessNonExplicitIdentifiers(node);

    private bool AliasedSourceHasColumn(FullyQualifiedName alias, FqnBuilder columnIdParts, TableSource sources)
        => sources.TryFindAlias(alias, out var parent) && SourceHasColumn(parent, columnIdParts);

    // If a vaid identifier is not in the name map then the normal id resolution
    // process will not be able to handle it. This method checks if such names
    // are either implicitly associated with or, qualified by an alias which can
    // be resolved back to, a known database object. In which case the
    // identifier can be ignored (it's not unresolved and the parent will have
    // been specified elsewhere in the token stream, thus its reference will be recorded.
    private void CheckIdentifierResolution(TableSource sources, FqnBuilder idParts)
    {
        if (!db.ContainsKey(idParts.Id))
        {
            var parent = idParts.Id.ShiftRight();

            if (parent == FullyQualifiedName.None)
            {
                if (UnqualifiedColumnHasParent(sources, idParts))
                {
                    ignore.Add(idParts.Id);
                }
            }
            else if (SourceHasColumn(parent, idParts))
            {
                ignore.Add(idParts.Id);
            }
            else if (AliasedSourceHasColumn(parent, idParts, sources))
            {
                ignore.Add(idParts.Id);
                ignore.Add(parent);
                ignore.Add(FullyQualifiedName.FromDbSchema(idParts.Id.Db, idParts.Id.Schema));
            }
        }
    }

    private void ProcessNonExplicitIdentifiers(TSqlFragment node)
    {
        TableSource sources = new(node.FragmentStream());
        FqnBuilder idParts = new();

        foreach (var token in node.FragmentStream())
        {
            idParts.AddToken(token);

            if (idParts.IsReady)
            {
                CheckIdentifierResolution(sources, idParts);
                idParts = new();
            }
        }
    }

    private bool SourceHasColumn(FullyQualifiedName parent, FqnBuilder columnIdParts)
        => root.Constituents.Contains(parent) || 
        (db.TryGetValue(parent, out var dbo) && dbo.Constituents.Any(c => c == FullyQualifiedName.FromName(columnIdParts.Id.Name)));


    private bool UnqualifiedColumnHasParent(TableSource sources, FqnBuilder idParts) 
        => sources.Sources.Any(s => SourceHasColumn(s, idParts));
}
