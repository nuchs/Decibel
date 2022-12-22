using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

internal sealed class FqnBuilder
{
    private readonly List<CaseInsensitiveString> parts = new();

    private bool isDb;
    private bool isSchema;

    internal FullyQualifiedName Id { get; private set; } = FullyQualifiedName.None;

    internal bool IsReady => Id != FullyQualifiedName.None;

    internal void AddToken(TSqlParserToken token)
    {
        if (IsReady)
        {
            throw new InvalidOperationException("Cannot add token - Identifier has already been built");
        }

        switch (token.TokenType)
        {
            case TSqlTokenType.Identifier:
            case TSqlTokenType.QuotedIdentifier:
                parts.Add(token.Text.Trim('[', ']'));
                break;

            case TSqlTokenType.Schema:
                isSchema = true;
                break;

            case TSqlTokenType.Database:
                isDb = true;
                break;

            case TSqlTokenType.Dot:
                break;

            default:
                if (parts.Count > 0)
                {
                    Build();
                }
                break;
        }
    }

    private void Build()
    {
        if (isDb)
        {
            Id = parts.Count switch
            {
                1 => FullyQualifiedName.FromDb(parts[0]),
                _ => throw new InvalidDataException($"Database names should consist of 1 part, got {parts.Count}")
            };
        }
        else if (isSchema)
        {
            Id = parts.Count switch
            {
                1 => FullyQualifiedName.FromSchema(parts[0]),
                2 => FullyQualifiedName.FromDbSchema(parts[0], parts[1]),
                _ => throw new InvalidDataException($"Schema names should consist of 1-2 parts, got {parts.Count}")
            };
        }
        else
        {
            Id = parts.Count switch
            {
                1 => FullyQualifiedName.FromName(parts[0]),
                2 => FullyQualifiedName.FromSchemaName(parts[0], parts[1]),
                3 => new(parts[0], parts[1], parts[2]),
                _ => throw new InvalidDataException($"Object names should consist of 1-3 parts, got {parts.Count}")
            };
        }
    }
}
