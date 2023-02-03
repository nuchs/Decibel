using Mast;
using Mast.Dbo;
using System.Text;

namespace Patchwerk.Delta;

internal sealed class TableDelta : DboDelta<Table>
{
    public TableDelta()
        : base("Table")
    {
    }

    /* As with views tables can reference others tables (via foreign keys), to
     * ensure the patches are able to be applied correctly the references must
     * be dropped, the referred to tables must be patched and the references
     * recreated before the referencing tables are updated
     *
     * As with views we can relations between tables form a partial order and
     * so we can use Kahn's algorithm to sort the tables into the order they
     * should be patched in
    */
    internal override IEnumerable<string> GenerateAddsAndUpdates(IDatabase before, IDatabase after)
    {
        Kahn<FullyQualifiedName, string> sorter = new();

        foreach (var table in after.Tables)
        {
            var candidate = before.Tables.FirstOrDefault(t => t.Identifier == table.Identifier);

            if (candidate is null)
            {
                sorter.AddNode(table.Identifier, table.Content, table.ReferencedBy.Select(r => r.Identifier));
            }
            else if (candidate != table)
            {
                // Mega complicated case
                sorter.AddNode(
                    table.Identifier,
                    TableDiff(table),
                    table.ReferencedBy.Select(r => r.Identifier));
            }
        }

        return sorter.Sort();
    }

    protected override IEnumerable<DbObject> Selector(IDatabase db)
        => db.Tables;

    private static string TableDiff(Table table)
    {
        StringBuilder patch = new();

        return patch.ToString();
    }
}
