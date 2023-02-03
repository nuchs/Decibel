using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class Column : DbFragment
{
    public Column(ColumnDefinition colDef)
            : base(colDef)
    {
        Name = GetName(colDef);
        DataType = GetTypeId(colDef.DataType);
        Nullable = GetNullability(colDef);
        PrimaryKey = GetPrimaryKey(colDef);
        Unique = GetUniqueness(colDef);
        Check = GetCheck(colDef);
        Default = GetDefault(colDef);
        Identity = GetIdentity(colDef);
        ForeignKey = GetForeignKey(colDef);
        Index = GetIndex(colDef);
    }

    public CheckConstraint? Check { get; }

    public FullyQualifiedName DataType { get; }

    public DefaultConstraint? Default { get; }

    public ForeignKey? ForeignKey { get; }

    public IdentityConstraint? Identity { get; }

    public Index? Index { get; }

    public string Name { get; }

    public NullConstraint? Nullable { get; }

    public PrimaryKey? PrimaryKey { get; }

    public UniqueConstraint? Unique { get; }

    internal IEnumerable<FullyQualifiedName> Constituents => new List<FullyQualifiedName>
    {
        FullyQualifiedName.FromName(Name),
        FullyQualifiedName.FromName(Check?.Name ?? string.Empty),
        FullyQualifiedName.FromName(Default?.Name ?? string.Empty),
        FullyQualifiedName.FromName(ForeignKey?.Name ?? string.Empty),
        FullyQualifiedName.FromName(PrimaryKey?.Name ?? string.Empty),
        FullyQualifiedName.FromName(Unique?.Name ?? string.Empty),
        FullyQualifiedName.FromName(Nullable?.Name ?? string.Empty),
        FullyQualifiedName.FromName(Index?.Name ?? string.Empty),
    };

    private static NullConstraint? GetNullability(ColumnDefinition colDef)
    {
        var nullConstraint = colDef
            .Constraints.OfType<NullableConstraintDefinition>()
            .FirstOrDefault();

        return nullConstraint is not null ? new(nullConstraint) : null;
    }

    private CheckConstraint? GetCheck(ColumnDefinition colDef)
    {
        var check = colDef.Constraints.OfType<CheckConstraintDefinition>().FirstOrDefault();

        return check is not null ? new(check) : null;
    }

    private DefaultConstraint? GetDefault(ColumnDefinition colDef)
        => colDef.DefaultConstraint is not null ? new(colDef.DefaultConstraint) : null;

    private ForeignKey? GetForeignKey(ColumnDefinition colDef)
    {
        var fk = colDef.Constraints.OfType<ForeignKeyConstraintDefinition>().FirstOrDefault();

        return fk is not null ? new(this, fk) : null;
    }

    private IdentityConstraint? GetIdentity(ColumnDefinition colDef)
        => colDef.IdentityOptions is not null ? new(colDef.IdentityOptions) : null;

    private Index? GetIndex(ColumnDefinition colDef) 
        => colDef.Index is not null ? new(new[] { this }, colDef.Index) : null;

    private string GetName(ColumnDefinition colDef)
        => GetId(colDef.ColumnIdentifier);

    private PrimaryKey? GetPrimaryKey(ColumnDefinition colDef)
    {
        var primaryConstraint = colDef.Constraints.OfType<UniqueConstraintDefinition>().Where(uq => uq.IsPrimaryKey).FirstOrDefault();

        return primaryConstraint is not null ? new(this, primaryConstraint) : null;
    }

    private FullyQualifiedName GetTypeId(DataTypeReference dataType)
        => FullyQualifiedName.FromSchemaName(GetId(dataType.Name.SchemaIdentifier), GetId(dataType.Name.BaseIdentifier));

    private UniqueConstraint? GetUniqueness(ColumnDefinition colDef)
    {
        var unique = colDef.Constraints.OfType<UniqueConstraintDefinition>().Where(uq => !uq.IsPrimaryKey).FirstOrDefault();

        return unique is not null ? new(this, unique) : null;
    }
}
