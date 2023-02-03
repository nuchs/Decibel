using Mast;
using Mast.Dbo;

namespace Patchwerk.Delta;

internal sealed class TypeDelta : DboDelta<DbObject>
{
    public TypeDelta()
        : base("Type")
    {
    }

    internal override IEnumerable<string> GenerateAddsAndUpdates(IDatabase before, IDatabase after)
    {
        var (preIds, postIds) = GetIds(before, after);
        var types = SortAffectedTypes(before, after, preIds, postIds);

        return PatchAndRecreatDependencies(before, types);
    }

    protected override IEnumerable<DbObject> Selector(IDatabase db)
        => db.ScalarTypes.AsEnumerable<DbObject>().Concat(db.TableTypes);

    private void AddDependentNodes(IDatabase after, Kahn<FullyQualifiedName, DbObject> sorter, FullyQualifiedName id)
    {
        var dbo = after[id];
        var added = sorter.AddNode(id, dbo, dbo.ReferencedBy.Select(n => n.Identifier));

        if (added)
        {
            foreach (var referee in dbo.ReferencedBy)
            {
                if (referee is ScalarType or TableType)
                {
                    AddDependentNodes(after, sorter, referee.Identifier);
                }
            }
        }
    }

    private string GetDependencyType(FullyQualifiedName id, DbObject depend)
    => depend switch
    {
        ScalarType _ => throw new InvalidOperationException($"{depend.Identifier} should have been processed prior to {id}. This code should be unreachable"),
        TableType _ => throw new InvalidOperationException($"{depend.Identifier} should have been processed prior to {id}. This code should be unreachable"),
        Table _ => throw new InvalidOperationException($"Type {id} is referenced by table {depend.Identifier} and cannot be modified"),
        View _ => "VIEW",
        Function _ => "FUNCTION",
        StoredProcedure _ => "PROCEDURE",
        Trigger _ => "TRIGGER",
        _ => throw new InvalidOperationException($"Cannot determine database type of {depend.Identifier} a dependency for type {id}")
    };

    private IEnumerable<string> PatchAndRecreatDependencies(IDatabase before, IEnumerable<DbObject> types)
    {
        List<string> drops = new();
        List<string> recreates = new();
        HashSet<FullyQualifiedName> done = new();

        foreach (var type in types.Reverse())
        {
            foreach (var depend in type.ReferencedBy)
            {
                if (done.Contains(depend.Identifier) || !before.ContainsKey(depend.Identifier))
                {
                    continue;
                }

                drops.Add($"DROP {GetDependencyType(type.Identifier, depend)} {depend.Identifier}");
                recreates.Add(depend.Content);
                done.Add(depend.Identifier);
            }

            if (before.ContainsKey(type.Identifier))
            {
                drops.Add($"DROP TYPE {type.Identifier}");
            }

            recreates.Add(type.Content);
            done.Add(type.Identifier);
        }

        recreates.Reverse();

        return drops.Concat(recreates);
    }

    private IEnumerable<DbObject> SortAffectedTypes(IDatabase before, IDatabase after,
                        IEnumerable<FullyQualifiedName> preIds,
        IEnumerable<FullyQualifiedName> postIds)
    {
        var adds = postIds.Except(preIds);
        var updates = postIds.Intersect(preIds).Where(id => before[id] != after[id]);
        Kahn<FullyQualifiedName, DbObject> sorter = new();

        foreach (var id in adds.Concat(updates))
        {
            AddDependentNodes(after, sorter, id);
        }

        return sorter.Sort();
    }
}
