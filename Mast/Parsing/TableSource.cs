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

    private readonly HashSet<FullyQualifiedName> sources = new();

    private readonly HashSet<TSqlTokenType> sourceSeparatorTokens = new()
    {
        TSqlTokenType.Comma,
        TSqlTokenType.Inner,
        TSqlTokenType.Join,
        TSqlTokenType.Left,
        TSqlTokenType.Outer,
        TSqlTokenType.Right,
        TSqlTokenType.RightOuterJoin,
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
                    ProcessStateNone(ref state, token);
                    break;

                case State.From:
                    ProcessStateFrom(ref state, ref id, token);
                    break;

                case State.Table:
                    ProcessStateTable(ref state, ref table, id, token);
                    break;

                case State.Space:
                    ProcessStateSpace(ref state, ref id, token);
                    break;

                case State.Alias:
                    ProcessStateAlias(ref state, ref table, id, token);
                    break;

                case State.Conditions:
                    ProcessStateConditions(ref state, token);
                    break;
            }
        }

        // Handle case where last token is final part of an identifier
        if (state == State.Alias)
        {
            id.Build();
            aliases[id.Id] = table;
        }

        if (state == State.Table || state == State.From)
        {
            id.Build();

            if (id.IsReady)
            {
                sources.Add(id.Id); 
            }
        }
    }

    internal IReadOnlyCollection<FullyQualifiedName> Sources => sources;

    internal bool TryFindAlias(FullyQualifiedName alias, [MaybeNullWhen(false)] out FullyQualifiedName fqn)
        => aliases.TryGetValue(alias, out fqn);

    private void ProcessStateAlias(ref State state, ref FullyQualifiedName table, FqnBuilder id, TSqlParserToken token)
    {
        id.AddToken(token);

        if (id.IsReady)
        {
            aliases[id.Id] = table;
            table = FullyQualifiedName.None;

            if (sourceSeparatorTokens.Contains(token.TokenType))
            {
                state = State.From;
            }
            else if (fromClauseTokens.Contains(token.TokenType))
            {
                state = State.Conditions;
            }
            else
            {
                state = State.None;
            }
        }
    }

    private void ProcessStateConditions(ref State state, TSqlParserToken token)
    {
        if (sourceSeparatorTokens.Contains(token.TokenType))
        {
            state = State.From;
        }
        else if (!fromClauseTokens.Contains(token.TokenType))
        {
            state = State.None;
        }
    }

    private void ProcessStateFrom(ref State state, ref FqnBuilder id, TSqlParserToken token)
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

    private void ProcessStateNone(ref State state, TSqlParserToken token)
    {
        if (token.TokenType == TSqlTokenType.From)
        {
            state = State.From;
        }
    }

    private void ProcessStateSpace(ref State state, ref FqnBuilder id, TSqlParserToken token)
    {
        if (identifierTokens.Contains(token.TokenType))
        {
            id = new();
            id.AddToken(token);
            state = State.Alias;
        }
        else if (token.TokenType != TSqlTokenType.WhiteSpace)
        {
            if (sourceSeparatorTokens.Contains(token.TokenType))
            {
                state = State.From;
            }
            else if (fromClauseTokens.Contains(token.TokenType))
            {
                state = State.Conditions;
            }
            else
            {
                state = State.None;
            }
        }
    }

    private void ProcessStateTable(ref State state, ref FullyQualifiedName table, FqnBuilder id, TSqlParserToken token)
    {
        id.AddToken(token);
        if (id.IsReady)
        {
            sources.Add(table);

            if (token.TokenType == TSqlTokenType.WhiteSpace)
            {
                table = id.Id;
                state = State.Space;
            }
            else if (sourceSeparatorTokens.Contains(token.TokenType))
            {
                state = State.From;
            }
            else if (fromClauseTokens.Contains(token.TokenType))
            {
                state = State.Conditions;
            }
            else
            {
                state = State.None;
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
        Conditions,
    }
}
