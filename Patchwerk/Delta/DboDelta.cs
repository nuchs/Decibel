using Mast;
using Mast.Dbo;

namespace Patchwerk.Delta;

internal abstract class DboDelta<T> where T : DbObject
{
    private readonly string type;

    protected DboDelta(string type) => this.type = type;

    internal virtual IEnumerable<string> GenerateAddsAndUpdates(IDatabase before, IDatabase after)
    {
        var preIds = Selector(before);
        var postIds = Selector(after);
        List<string> patches = new();

        patches.AddRange(AddNewObjects(after, preIds, postIds));
        patches.AddRange(PatchChangedObjects(before, after, preIds, postIds));

        return patches;
    }

    internal IEnumerable<string> GenerateDrops(IDatabase before, IDatabase after)
    {
        var preIds = Selector(before);
        var postIds = Selector(after);

        return preIds.Except(postIds).Select(id => before[id]).Select(dbo => $"DROP {type.ToUpper()} {dbo.Identifier}");
    }

    protected abstract string Delta(T pre, T post);

    protected abstract IEnumerable<FullyQualifiedName> Selector(IDatabase db);

    private IEnumerable<string> AddNewObjects(IDatabase after, IEnumerable<FullyQualifiedName> preIds, IEnumerable<FullyQualifiedName> postIds)
        => postIds.Except(preIds).Select(id => after[id].Content);

    private string GenerateDiff(IDatabase before, IDatabase after, FullyQualifiedName dboId)
    {
        if (before[dboId] is T preObj && after[dboId] is T postObj)
        {
            return Delta(preObj, postObj);
        }
        else
        {
            throw new InvalidCastException($"Bad id {dboId} does not refer to a {type.ToLower()}");
        }
    }

    private IEnumerable<string> PatchChangedObjects(IDatabase before, IDatabase after, IEnumerable<FullyQualifiedName> preIds, IEnumerable<FullyQualifiedName> postIds) 
        => preIds.Intersect(postIds).Where(id => before[id] != after[id]).Select(id => GenerateDiff(before, after, id));
}
