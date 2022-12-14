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
        Default = GetDefault(colDef);
        Identity = GetIdentity(colDef);
    }

    public CheckConstraint? Check { get; }

    public ScalarType DataType { get; }

    public DefaultConstraint? Default { get; }

    public IdentityConstraint? Identity { get; }

    public bool IsNullable { get; }

    public PrimaryKey? PrimaryKey { get; }

    public UniqueConstraint? Unique { get; }

    private static bool GetNullability(ColumnDefinition colDef)
    {
        var nullConstrints = colDef
            .Constraints.OfType<NullableConstraintDefinition>()
            .Select(n => n.Nullable);

        return !nullConstrints.Any() || nullConstrints.First();
    }

    private CheckConstraint? GetCheck(ColumnDefinition colDef)
    {
        var check = colDef.Constraints.OfType<CheckConstraintDefinition>().FirstOrDefault();

        return check is not null ? new CheckConstraint(check) : null;
    }

    private DefaultConstraint? GetDefault(ColumnDefinition colDef) 
        => colDef.DefaultConstraint is not null ? new(colDef.DefaultConstraint) : null;

    private IdentityConstraint? GetIdentity(ColumnDefinition colDef) 
        => colDef.IdentityOptions is not null ? new(colDef.IdentityOptions) : null;

    private string GetName(ColumnDefinition colDef)
        => GetId(colDef.ColumnIdentifier);

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
}
