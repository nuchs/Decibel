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
                sorter.AddNode(
                    table.Identifier,
                    TableDiff(candidate, table),
                    table.ReferencedBy.Select(r => r.Identifier));
            }
        }

        return sorter.Sort();
    }

    protected override IEnumerable<DbObject> Selector(IDatabase db)
        => db.Tables;

    private static string TableDiff(Table old, Table current)
    {
        StringBuilder patch = new();

        var newColNames = current.Columns.Select(c => c.Name);
        var oldColNames = old.Columns.Select(c => c.Name);

        var drops = oldColNames.Except(newColNames);
        var adds = newColNames.Except(oldColNames).Select(name => current.Columns.First(c => c.Name == name));
        var updates = newColNames
            .Intersect(oldColNames)
            .Where(name => current.Columns.First(c => c.Name == name) != old.Columns.First(c => c.Name == name))
            .Select(name => current.Columns.First(c => c.Name == name));



        var currentRefs = current.ReferencedBy.OfType<Table>().Select(t => t.Identifier);
        var continuingRefs = old.ReferencedBy.OfType<Table>().Where(t => currentRefs.Contains(t.Identifier));
        var changedColumns = current.Columns.Where(old.Columns.Contains);

        return patch.ToString();
    }
}
