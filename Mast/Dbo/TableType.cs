using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class TableType : DbObject
{
    public TableType(CreateTypeTableStatement node)
        : base(node)
    {
        Identifier = AssembleIdentifier(node);
        Columns = CollectColumns(node);
        Indices = CollectIndices(node);
        Checks = GetChecks(node);
        UniqueConstraints = GetUniqueConstraints(node);
        PrimaryKey = GetPrimary(node);
    }

    public IEnumerable<CheckConstraint> Checks { get; }

    public IEnumerable<Column> Columns { get; }

    public IEnumerable<Index> Indices { get; }

    public PrimaryKey? PrimaryKey { get; }

    public IEnumerable<UniqueConstraint> UniqueConstraints { get; }

    internal override IEnumerable<FullyQualifiedName> Constituents
        => base.Constituents
            .Concat(Columns.Select(c => FullyQualifiedName.FromName(c.Name)))
            .Concat(Indices.Select(i => FullyQualifiedName.FromName(i.Name)));

    private FullyQualifiedName AssembleIdentifier(CreateTypeTableStatement node)
        => FullyQualifiedName.FromSchemaName(GetId(node.Name.SchemaIdentifier), GetId(node.Name.BaseIdentifier));

    private IEnumerable<Column> CollectColumns(CreateTypeTableStatement node)
        => node.Definition.ColumnDefinitions.Select(c => new Column(c));

    private IEnumerable<Index> CollectIndices(CreateTypeTableStatement node)
        => node.Definition.Indexes.Select(i => new Index(Columns, i));

    private IEnumerable<CheckConstraint> GetChecks(CreateTypeTableStatement table)
        => table
            .Definition
            .TableConstraints
            .OfType<CheckConstraintDefinition>()
            .Select(c => new CheckConstraint(c));

    private PrimaryKey? GetPrimary(CreateTypeTableStatement node)
    {
        var primaryCol = Columns.Select(c => c.PrimaryKey).FirstOrDefault(p => p is not null);

        if (primaryCol is not null)
        {
            return primaryCol;
        }

        var compoundPrimary = node.Definition.TableConstraints.OfType<UniqueConstraintDefinition>().FirstOrDefault(uq => uq.IsPrimaryKey);

        if (compoundPrimary is not null)
        {
            return new(Columns, compoundPrimary);
        }

        return null;
    }

    private IEnumerable<UniqueConstraint> GetUniqueConstraints(CreateTypeTableStatement table)
       => table
           .Definition
           .TableConstraints
           .OfType<UniqueConstraintDefinition>()
           .Select(u => new UniqueConstraint(Columns, u));
}
