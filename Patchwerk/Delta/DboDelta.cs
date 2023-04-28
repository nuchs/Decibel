using Mast;
using Mast.Dbo;

namespace Patchwerk.Delta;

internal abstract class DboDelta<T> where T : DbObject
{
    private readonly string type;

    protected DboDelta(string type) => this.type = type;

    internal virtual IEnumerable<string> GenerateAddsAndUpdates(IDatabase before, IDatabase after)
    {
        var (preIds, postIds) = GetIds(before, after);
        List<string> patches = new();

        patches.AddRange(AddNewObjects(after, preIds, postIds));
        patches.AddRange(PatchChangedObjects(before, after, preIds, postIds));

        return patches;
    }

    internal IEnumerable<string> GenerateDrops(IDatabase before, IDatabase after)
    {
        var (preIds, postIds) = GetIds(before, after);

        return preIds.Except(postIds).Select(id => before[id]).Select(dbo => $"DROP {type.ToUpper()} {dbo.Identifier}");
    }

    protected virtual string Delta(T pre, T post)
            => $"""
        DROP {type.ToUpper()} {pre.Identifier}
        GO

        {post.Content}
        """;

    protected (IEnumerable<FullyQualifiedName>, IEnumerable<FullyQualifiedName>) GetIds(IDatabase before, IDatabase after)
        => (Selector(before).Select(dbo => dbo.Identifier), Selector(after).Select(dbo => dbo.Identifier));

    protected abstract IEnumerable<DbObject> Selector(IDatabase db);

    private IEnumerable<string> AddNewObjects(IDatabase after, IEnumerable<FullyQualifiedName> preIds, IEnumerable<FullyQualifiedName> postIds)
        => postIds.Except(preIds).Select(id => after[id].Content);

    private IEnumerable<string> PatchChangedObjects(IDatabase before, IDatabase after, IEnumerable<FullyQualifiedName> preIds, IEnumerable<FullyQualifiedName> postIds)
    {
        foreach (var dboId in preIds.Intersect(postIds).Where(id => before[id] != after[id]))
        {
            if (before[dboId] is T preObj && after[dboId] is T postObj)
            {
                yield return Delta(preObj, postObj);
            }
            else
            {
                throw new InvalidCastException($"Bad id {dboId} does not refer to a {type.ToLower()}");
            }
        }
    }
}
