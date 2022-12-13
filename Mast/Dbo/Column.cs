using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class Column : DbObject
{
    public Column(ColumnDefinition colDef)
        : base(colDef)
    {
        Name = GetName(colDef);
        DataType = new ScalarType(colDef.DataType);
        IsNullable = GetNullability(colDef);
        PrimaryKey = GetPrimaryKey(colDef);
        IsUnique = GetUniqueness(colDef);

        if (HasDefault(colDef))
        {
            DefaultName = GetDefaultName(colDef);
            Default = AssembleDefaultValue(colDef);
        }

        if (HasIdentity(colDef))
        {
            Identity = new(colDef.IdentityOptions);
        }
    }

    public ScalarType DataType { get; }

    public string? Default { get; }

    public string? DefaultName { get; }

    public Identity? Identity { get; }

    public bool IsNullable { get; }

    public PrimaryKey? PrimaryKey { get; }

    public bool IsUnique { get; }

    private static string? GetDefaultName(ColumnDefinition colDef)
        => colDef.DefaultConstraint.ConstraintIdentifier?.Value;

    private static string GetName(ColumnDefinition colDef)
        => colDef.ColumnIdentifier.Value;

    private static bool GetNullability(ColumnDefinition colDef)
    {
        var nullConstrints = colDef
            .Constraints.OfType<NullableConstraintDefinition>()
            .Select(n => n.Nullable);

        return !nullConstrints.Any() || nullConstrints.First();
    }

    private string AssembleDefaultValue(ColumnDefinition colDef) 
        => AssembleFragment(
            colDef.DefaultConstraint, 
            colDef.DefaultConstraint.FirstTokenIndex + 1, 
            colDef.DefaultConstraint.LastTokenIndex + 1);

    private PrimaryKey? GetPrimaryKey(ColumnDefinition colDef)
    {
        var primaryConstraint = colDef.Constraints.OfType<UniqueConstraintDefinition>().Where(uq => uq.IsPrimaryKey).FirstOrDefault();

        return primaryConstraint is not null ? new PrimaryKey(this, primaryConstraint) : null;
    }

    private bool GetUniqueness(ColumnDefinition colDef)
        => colDef.Constraints.OfType<UniqueConstraintDefinition>().Any();

    private bool HasDefault(ColumnDefinition colDef) => colDef.DefaultConstraint is not null;

    private bool HasIdentity(ColumnDefinition colDef) => colDef.IdentityOptions is not null;
}
