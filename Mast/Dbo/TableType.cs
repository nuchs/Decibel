using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class TableType : DbObject
{
    public TableType(CreateTypeTableStatement node)
        : base(node)
    {
        Schema = GetSchema(node);
        Name = GetName(node);
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

    public string Schema { get; }

    public IEnumerable<UniqueConstraint> UniqueConstraints { get; }

    private static IEnumerable<Column> CollectColumns(CreateTypeTableStatement node)
        => node.Definition.ColumnDefinitions.Select(c => new Column(c));

    private IEnumerable<Index> CollectIndices(CreateTypeTableStatement node)
        => node.Definition.Indexes.Select(i => new Index(Columns, i));

    private IEnumerable<CheckConstraint> GetChecks(CreateTypeTableStatement table)
        => table
            .Definition
            .TableConstraints
            .OfType<CheckConstraintDefinition>()
            .Select(c => new CheckConstraint(c));

    private string GetName(CreateTypeTableStatement node)
            => GetId(node.Name.BaseIdentifier);

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

    private string GetSchema(CreateTypeTableStatement node)
        => GetId(node.Name.SchemaIdentifier);

    private IEnumerable<UniqueConstraint> GetUniqueConstraints(CreateTypeTableStatement table)
       => table
           .Definition
           .TableConstraints
           .OfType<UniqueConstraintDefinition>()
           .Select(u => new UniqueConstraint(Columns, u));
}
