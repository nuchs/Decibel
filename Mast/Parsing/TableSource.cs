using Mast.Dbo;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Diagnostics.CodeAnalysis;

namespace Mast.Parsing;

internal class TableSource
{
    private readonly Dictionary<FullyQualifiedName, FullyQualifiedName> aliases = new();

    private readonly HashSet<TSqlTokenType> fromClauseTokens = new()
    {
        TSqlTokenType.As,
        TSqlTokenType.AsciiStringLiteral,
        TSqlTokenType.AsciiStringOrQuotedIdentifier,
        TSqlTokenType.Comma,
        TSqlTokenType.Cross,
        TSqlTokenType.CurrentDate,
        TSqlTokenType.CurrentTime,
        TSqlTokenType.CurrentTimestamp,
        TSqlTokenType.Dot,
        TSqlTokenType.EqualsSign,
        TSqlTokenType.Full,
        TSqlTokenType.Identifier,
        TSqlTokenType.Inner,
        TSqlTokenType.Is,
        TSqlTokenType.Join,
        TSqlTokenType.Left,
        TSqlTokenType.LeftParenthesis,
        TSqlTokenType.MultilineComment,
        TSqlTokenType.Not,
        TSqlTokenType.Null,
        TSqlTokenType.Numeric,
        TSqlTokenType.On,
        TSqlTokenType.Or,
        TSqlTokenType.Outer,
        TSqlTokenType.QuotedIdentifier,
        TSqlTokenType.Right,
        TSqlTokenType.RightOuterJoin,
        TSqlTokenType.RightParenthesis,
        TSqlTokenType.SingleLineComment,
        TSqlTokenType.Variable,
        TSqlTokenType.WhiteSpace,
    };

    private readonly HashSet<TSqlTokenType> identifierTokens = new()
    {
        TSqlTokenType.Identifier,
        TSqlTokenType.QuotedIdentifier,
        TSqlTokenType.Dot,
        TSqlTokenType.Variable,
    };

    internal TableSource(IEnumerable<TSqlParserToken> tokenStream)
    {
        var state = State.None;
        var table = FullyQualifiedName.None;
        FqnBuilder id = new();

        foreach (var token in tokenStream)
        {
            switch (state)
            {
                case State.None:
                    ProcessTokenStateNone(ref state, token);
                    break;

                case State.From:
                    ProcessTokenStateFrom(ref state, ref id, token);
                    break;

                case State.Table:
                    ProcessTokenStateTable(ref state, ref table, id, token);
                    break;

                case State.Space:
                    ProcessTokenStateSpace(ref state, ref id, token);
                    break;

                case State.Alias:
                    ProcessTokenStateAlias(ref state, table, id, token);
                    break;
            }
        }

        // Handle case where last token is final part of an identifier
        if (state == State.Alias)
        {
            id.Build();
            aliases[id.Id] = table;
        }
    }

    internal bool TryFindAlias(FullyQualifiedName alias, [MaybeNullWhen(false)] out FullyQualifiedName fqn)
        => aliases.TryGetValue(alias, out fqn);

    internal bool TryFindParent(FullyQualifiedName child, [MaybeNullWhen(false)] out FullyQualifiedName parent)
    {
        parent = FullyQualifiedName.None;
        return false;
    }

    private void ProcessTokenStateAlias(ref State state, FullyQualifiedName table, FqnBuilder id, TSqlParserToken token)
    {
        id.AddToken(token);

        if (id.IsReady)
        {
            aliases[id.Id] = table;
            state = fromClauseTokens.Contains(token.TokenType) ? State.From : State.None;
        }
    }

    private void ProcessTokenStateFrom(ref State state, ref FqnBuilder id, TSqlParserToken token)
    {
        if (identifierTokens.Contains(token.TokenType))
        {
            id = new();
            id.AddToken(token);
            state = State.Table;
        }
        else if (!fromClauseTokens.Contains(token.TokenType))
        {
            state = State.None;
        }
    }

    private void ProcessTokenStateNone(ref State state, TSqlParserToken token)
    {
        if (token.TokenType == TSqlTokenType.From)
        {
            state = State.From;
        }
    }

    private void ProcessTokenStateSpace(ref State state, ref FqnBuilder id, TSqlParserToken token)
    {
        if (identifierTokens.Contains(token.TokenType))
        {
            id = new();
            id.AddToken(token);
            state = State.Alias;
        }
        else if (token.TokenType != TSqlTokenType.WhiteSpace)
        {
            state = fromClauseTokens.Contains(token.TokenType) ? State.From : State.None;
        }
    }

    private void ProcessTokenStateTable(ref State state, ref FullyQualifiedName table, FqnBuilder id, TSqlParserToken token)
    {
        id.AddToken(token);
        if (id.IsReady)
        {
            if (token.TokenType == TSqlTokenType.WhiteSpace)
            {
                table = id.Id;
                state = State.Space;
            }
            else
            {
                state = fromClauseTokens.Contains(token.TokenType) ? State.From : State.None;
            }
        }
    }

    private enum State
    {
        None,
        From,
        Table,
        Space,
        Alias,
    }
}
