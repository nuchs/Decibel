using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class Column
{
    public Column(ColumnDefinition colDef)
    {
        Name = GetName(colDef);
        DataType = new ScalarType(colDef.DataType);
        IsNullable = GetNullability(colDef);
        IsPrimary = GetPrimary(colDef);
        IsUnique = GetUniqueness(colDef);
        Content = AssembleColumnValue(colDef);

        if (HasDefault(colDef))
        {
            DefaultName = GetDefaultName(colDef);
            Default = AssembleDefaultValue(colDef);
        }

        if (HasIdentity(colDef))
        {
            Identity = new(
                AssembleIdentitySeed(colDef.IdentityOptions),
                AssembleIdentityIncrement(colDef.IdentityOptions));
        }
    }

    public string Content { get; }

    public ScalarType DataType { get; }

    public string? Default { get; }

    public string? DefaultName { get; }

    public Identity? Identity { get; }

    public bool IsNullable { get; }

    public bool IsPrimary { get; }

    public bool IsUnique { get; }

    public string Name { get; }

    public override string ToString() => Content;

    private static string AssembleColumnValue(ColumnDefinition colDef)
    {
        var tokenValues = colDef
            .ScriptTokenStream
            .Take(colDef.FirstTokenIndex..(colDef.LastTokenIndex + 1))
            .Select(t => t.Text);

        return string.Join(string.Empty, tokenValues);
    }

    private static int AssembleIdentityIncrement(IdentityOptions id)
    {
        if (id.IdentityIncrement is null)
        {
            return 1;
        }

        var incrementTokens = id
          .IdentityIncrement.ScriptTokenStream
          .Take(id.IdentityIncrement.FirstTokenIndex..(id.IdentityIncrement.LastTokenIndex + 1))
          .Select(t => t.Text);

        return int.Parse(string.Join(string.Empty, incrementTokens));
    }

    private static int AssembleIdentitySeed(IdentityOptions id)
    {
        if (id.IdentitySeed is null)
        {
            return 1;
        }

        var seedTokens = id
          .IdentitySeed.ScriptTokenStream
          .Take(id.IdentitySeed.FirstTokenIndex..(id.IdentitySeed.LastTokenIndex + 1))
          .Select(t => t.Text);

        return int.Parse(string.Join(string.Empty, seedTokens));
    }

    private static string? GetDefaultName(ColumnDefinition colDef)
        => colDef.DefaultConstraint.ConstraintIdentifier?.Value;

    private static string GetName(ColumnDefinition colDef)
        => colDef.ColumnIdentifier.Value;

    private static bool GetNullability(ColumnDefinition colDef)
    {
        var nullConstrints = colDef
            .Constraints.OfType<NullableConstraintDefinition>()
            .Select(n => n.Nullable);

        return nullConstrints.Any() ? nullConstrints.First() : true;
    }

    private string AssembleDefaultValue(ColumnDefinition colDef)
    {
        var dfltTokens = colDef
            .DefaultConstraint
            .ScriptTokenStream
            .Take((colDef.DefaultConstraint.FirstTokenIndex + 1)..(colDef.DefaultConstraint.LastTokenIndex + 1))
            .Select(d => d.Text);

        return string.Join("", dfltTokens).Trim();
    }

    private bool GetPrimary(ColumnDefinition colDef)
        => colDef.Constraints.OfType<UniqueConstraintDefinition>().Where(uq => uq.IsPrimaryKey).Any();

    private bool GetUniqueness(ColumnDefinition colDef)
        => colDef.Constraints.OfType<UniqueConstraintDefinition>().Any();

    private bool HasDefault(ColumnDefinition colDef) => colDef.DefaultConstraint is not null;

    private bool HasIdentity(ColumnDefinition colDef) => colDef.IdentityOptions is not null;
}
