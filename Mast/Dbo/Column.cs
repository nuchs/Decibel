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
        Unique = GetUniqueness(colDef);
        Check = GetCheck(colDef);

        if (HasDefault(colDef))
        {
            Default = new(colDef.DefaultConstraint);
        }

        if (HasIdentity(colDef))
        {
            Identity = new(colDef.IdentityOptions);
        }
    }

    public ScalarType DataType { get; }

    public DefaultConstraint? Default { get; }

    public IdentityConstraint? Identity { get; }

    public bool IsNullable { get; }

    public PrimaryKey? PrimaryKey { get; }

    public UniqueConstraint? Unique { get; }

    public CheckConstraint? Check { get; } 

    private static string GetName(ColumnDefinition colDef)
        => colDef.ColumnIdentifier.Value;

    private static bool GetNullability(ColumnDefinition colDef)
    {
        var nullConstrints = colDef
            .Constraints.OfType<NullableConstraintDefinition>()
            .Select(n => n.Nullable);

        return !nullConstrints.Any() || nullConstrints.First();
    }

    private PrimaryKey? GetPrimaryKey(ColumnDefinition colDef)
    {
        var primaryConstraint = colDef.Constraints.OfType<UniqueConstraintDefinition>().Where(uq => uq.IsPrimaryKey).FirstOrDefault();

        return primaryConstraint is not null ? new PrimaryKey(this, primaryConstraint) : null;
    }

    private UniqueConstraint? GetUniqueness(ColumnDefinition colDef)
    {
        var unique = colDef.Constraints.OfType<UniqueConstraintDefinition>().Where(uq => !uq.IsPrimaryKey).FirstOrDefault();

        return unique is not null ? new UniqueConstraint(this, unique) : null;
    }

    private CheckConstraint? GetCheck(ColumnDefinition colDef)
    {
        var check = colDef.Constraints.OfType<CheckConstraintDefinition>().FirstOrDefault();

        return check is not null ? new CheckConstraint(check) : null;
    }

    private bool HasDefault(ColumnDefinition colDef) => colDef.DefaultConstraint is not null;

    private bool HasIdentity(ColumnDefinition colDef) => colDef.IdentityOptions is not null;
}
