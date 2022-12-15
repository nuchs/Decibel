using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class Column : DbFragment
{
    public Column(ColumnDefinition colDef)
        : base(colDef)
    {
        Name = GetName(colDef);
        DataType = GetTypeId(colDef.DataType);
        IsNullable = GetNullability(colDef);
        PrimaryKey = GetPrimaryKey(colDef);
        Unique = GetUniqueness(colDef);
        Check = GetCheck(colDef);
        Default = GetDefault(colDef);
        Identity = GetIdentity(colDef);
        ForeginKey = GetForeginKey(colDef);
    }

    public CheckConstraint? Check { get; }

    public FullyQualifiedName DataType { get; }

    public DefaultConstraint? Default { get; }

    public ForeginKey? ForeginKey { get; }

    public IdentityConstraint? Identity { get; }

    public bool IsNullable { get; }

    public string Name { get; }

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

        return check is not null ? new(check) : null;
    }

    private DefaultConstraint? GetDefault(ColumnDefinition colDef)
        => colDef.DefaultConstraint is not null ? new(colDef.DefaultConstraint) : null;

    private ForeginKey? GetForeginKey(ColumnDefinition colDef)
    {
        var fk = colDef.Constraints.OfType<ForeignKeyConstraintDefinition>().FirstOrDefault();

        return fk is not null ? new(this, fk) : null;
    }

    private IdentityConstraint? GetIdentity(ColumnDefinition colDef)
        => colDef.IdentityOptions is not null ? new(colDef.IdentityOptions) : null;

    private string GetName(ColumnDefinition colDef)
        => GetId(colDef.ColumnIdentifier);

    private PrimaryKey? GetPrimaryKey(ColumnDefinition colDef)
    {
        var primaryConstraint = colDef.Constraints.OfType<UniqueConstraintDefinition>().Where(uq => uq.IsPrimaryKey).FirstOrDefault();

        return primaryConstraint is not null ? new(this, primaryConstraint) : null;
    }

    private FullyQualifiedName GetTypeId(DataTypeReference dataType)
        => new(GetId(dataType.Name.SchemaIdentifier), GetId(dataType.Name.BaseIdentifier));

    private UniqueConstraint? GetUniqueness(ColumnDefinition colDef)
    {
        var unique = colDef.Constraints.OfType<UniqueConstraintDefinition>().Where(uq => !uq.IsPrimaryKey).FirstOrDefault();

        return unique is not null ? new(this, unique) : null;
    }
}
