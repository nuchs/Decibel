using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class Table : DbObject
{
    public Table(CreateTableStatement node)
        : base(node)
    {
        Identifier = AssembleIdentifier(node);
        Columns = CollectColumns(node);
        Indices = CollectIndices(node);
        Checks = GetChecks(node);
        UniqueConstraints = GetUniqueConstraints(node);
        PrimaryKey = GetPrimary(node);
        ForeignKeys = CollectForeignKeys(node);
    }

    public IEnumerable<CheckConstraint> Checks { get; }

    public IEnumerable<Column> Columns { get; }

    public IEnumerable<ForeignKey> ForeignKeys { get; }

    public IEnumerable<Index> Indices { get; }

    public PrimaryKey? PrimaryKey { get; }

    public IEnumerable<UniqueConstraint> UniqueConstraints { get; }

    internal override IEnumerable<FullyQualifiedName> Constituents
        => base.Constituents.Concat(Columns.Select(c => FullyQualifiedName.FromName(c.Name)));

    private FullyQualifiedName AssembleIdentifier(CreateTableStatement node)
    {
        return FullyQualifiedName.FromSchemaName(
            GetId(node.SchemaObjectName.SchemaIdentifier),
            GetId(node.SchemaObjectName.BaseIdentifier));
    }

    private IEnumerable<Column> CollectColumns(CreateTableStatement table)
            => table.Definition.ColumnDefinitions.Select(c => new Column(c));

    private IEnumerable<ForeignKey> CollectForeignKeys(CreateTableStatement table)
        => table
            .Definition
            .TableConstraints
            .OfType<ForeignKeyConstraintDefinition>()
            .Select(fk => new ForeignKey(Columns, fk));

    private IEnumerable<Index> CollectIndices(CreateTableStatement node)
        => node.Definition.Indexes.Select(i => new Index(Columns, i));

    private IEnumerable<CheckConstraint> GetChecks(CreateTableStatement table)
        => table
          .Definition
          .TableConstraints
          .OfType<CheckConstraintDefinition>()
          .Select(c => new CheckConstraint(c));

    private PrimaryKey? GetPrimary(CreateTableStatement table)
    {
        var primaryCol = Columns.Select(c => c.PrimaryKey).FirstOrDefault(p => p is not null);

        if (primaryCol is not null)
        {
            return primaryCol;
        }

        var compoundPrimary = table.Definition.TableConstraints.OfType<UniqueConstraintDefinition>().FirstOrDefault(uq => uq.IsPrimaryKey);

        if (compoundPrimary is not null)
        {
            return new(Columns, compoundPrimary);
        }

        return null;
    }

    private IEnumerable<UniqueConstraint> GetUniqueConstraints(CreateTableStatement table)
        => table
           .Definition
           .TableConstraints
           .OfType<UniqueConstraintDefinition>()
           .Select(u => new UniqueConstraint(Columns, u));
}
