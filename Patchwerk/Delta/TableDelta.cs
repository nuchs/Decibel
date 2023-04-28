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
     * As with views relations between tables form a partial order and so we
     * can use Kahn's algorithm to sort the tables into the order they should
     * be patched in
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

        var newColNames = current.Columns.Select(c => new CaseInsensitiveString(c.Name));
        var oldColNames = old.Columns.Select(c => new CaseInsensitiveString(c.Name));

        var drops = oldColNames.Except(newColNames);
        var adds = newColNames.Except(oldColNames).Select(name => current.Columns.First(c => c.Name == name));
        var updates = newColNames
            .Intersect(oldColNames)
            .Where(name => current.Columns.First(c => c.Name == name) != old.Columns.First(c => c.Name == name))
            .Select(name => current.Columns.First(c => c.Name == name));

        foreach (var add in adds)
        {
            if (!Nullable(add) && add.Default is null)
            {
                throw new InvalidOperationException("Cannot add a non null column without a default");
            }

            patch.AppendLine($"ALTER TABLE {current.Identifier} ADD {add.Content}\nGO\n\n");
        }

        foreach (var update in updates)
        {
            CheckColumnUpdate(old, update);
            patch.AppendLine($"ALTER TABLE {current.Identifier} ALTER COLUMN {update.Content}");
        }

        foreach (var drop in drops)
        {
            // TODO drop references
            var referees = old.ReferencedBy.OfType<Table>().Where(
                t => HasRefViaTableConstraint(old, t, drop) || HasRefViaColumnConstraint(old, t, drop));

            patch.AppendLine($"ALTER TABLE {current.Identifier} DROP COLUMN {drop}");
        }

        var currentRefs = current.ReferencedBy.OfType<Table>().Select(t => t.Identifier);
        var continuingRefs = old.ReferencedBy.OfType<Table>().Where(t => currentRefs.Contains(t.Identifier));
        var changedColumns = current.Columns.Where(old.Columns.Contains);

        return patch.ToString();
    }

    private static bool Nullable(Column add) => add.Nullable?.IsNullable ?? true;

    private static void CheckColumnUpdate(Table oldTable, Column newColumn)
    {
        StringBuilder errors = new();
        var oldColumn = oldTable.Columns.First(old => old.Name == newColumn.Name);

        if (oldColumn.DataType != newColumn.DataType)
        {
            errors.AppendLine($"""
                It is not safe to change the datatype of a column

                    {newColumn.Name} : {oldColumn.DataType} => {newColumn.DataType}

                It will automatically convert the data in the column to the new type or fail at runtime.
                If this is what you want then write a manual patch to do this.
                """);
        }

        if (Nullable(oldColumn) && !Nullable(newColumn) && newColumn.Default is null)
        {
            errors.AppendLine($"Cannot make nullable column {newColumn.Name} non nullable without a default");
        }

        if (oldColumn.Check != newColumn.Check)
        {
            errors.AppendLine($"Cannot change check constraint on column {newColumn.Name}, it must be added as a table constraint");
        }

        if (oldColumn.Default != newColumn.Default)
        {
            errors.AppendLine($"Cannot change default constraint to column {newColumn.Name}, it must be added as a table constraint");
        }

        if (oldColumn.ForeignKey != newColumn.ForeignKey)
        {
            errors.AppendLine($"Cannot change foreign key constraint to column {newColumn.Name}, it must be added as a table constraint");
        }

        if (oldColumn.PrimaryKey != newColumn.PrimaryKey)
        {
            errors.AppendLine($"Cannot change primary constraint to column {newColumn.Name}, it must be added as a table constraint");
        }

        if (oldColumn.Unique != newColumn.Unique)
        {
            errors.AppendLine($"Cannot change check constraint to column {newColumn.Name}, it must be added as a table constraint");
        }

        if (oldColumn.Identity != newColumn.Identity)
        {
            errors.AppendLine($"Cannot change check constraint to column {newColumn.Name}, it must be added as a table constraint");
        }

        if (errors.Length > 0)
        {
            throw new InvalidOperationException($"Error updating table {oldTable.Identifier}\n{errors}");
        }
    }

    private static bool HasRefViaColumnConstraint(Table referent, Table referee, CaseInsensitiveString column) 
        => referee.Columns.Any(c => c.ForeignKey is not null && c.ForeignKey.ForeignTable == referent.Identifier && c.ForeignKey.ForeignColumn == column);
    
    private static bool HasRefViaTableConstraint(Table referent, Table referee, CaseInsensitiveString column) 
        => referee.ForeignKeys.Any(fk => fk.ForeignTable == referent.Identifier && fk.ForeignColumn == column);
}
