using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class Index : DbObject
{
    public Index(IEnumerable<Column> tableColumns, IndexDefinition index)
        : base(index)
    {
        Name = GetName(index);
        Clustered = GetClustered(index);
        Columns = CollectColumns(index, tableColumns);
    }

    public bool Clustered { get; }

    public IEnumerable<IndexColumn> Columns { get; }

    private static bool GetClustered(IndexDefinition index)
        => index.IndexType.IndexTypeKind == IndexTypeKind.Clustered
        || index.IndexType.IndexTypeKind == IndexTypeKind.ClusteredColumnStore;

    private IEnumerable<IndexColumn> CollectColumns(IndexDefinition index, IEnumerable<Column> tableColumns)
    {
        var columns = new List<IndexColumn>();

        foreach (var col in index.Columns)
        {
            var idParts = col.Column.MultiPartIdentifier.Identifiers.Select(i => i.Value);
            var id = string.Join('.', idParts);
            var c = tableColumns.Where(t => t.Name == id);

            columns.Add(new(MapSortOrder(col), c.First()));
        }

        return columns;
    }

    private string GetName(IndexDefinition index)
        => GetId(index.Name);

    private Direction MapSortOrder(ColumnWithSortOrder col)
            => col.SortOrder switch
            {
                SortOrder.NotSpecified => Direction.NotSet,
                SortOrder.Ascending => Direction.Asc,
                SortOrder.Descending => Direction.Desc,
                _ => throw new InvalidDataException($"Unrecognised sort order {col.SortOrder}")
            };
}
