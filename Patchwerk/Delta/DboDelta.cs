using Mast;
using Mast.Dbo;

namespace Patchwerk.Delta;

internal abstract class DbDelta<T> where T : DbObject
{
    private readonly string type;

    protected DbDelta(string type) => this.type = type;

    internal void GeneratePatches(IDatabase before, IDatabase after, List<string> patches)
    {
        var preIds = Selector(before);
        var postIds = Selector(after);

        foreach (var id in postIds.Except(preIds))
        {
            var dbo = after[id];
            patches.Add(dbo.Content);
        }

        foreach (var id in preIds.Except(postIds))
        {
            var dbo = before[id];
            patches.Add($"DROP {type.ToUpper()} {dbo.Identifier}");
        }

        foreach (var dboId in preIds.Intersect(postIds))
        {
            if (before[dboId] is T preObj && after[dboId] is T postObj)
            {
                if (preObj != postObj)
                {
                    Delta(preObj, postObj, patches);
                }
            }
            else
            {
                throw new InvalidCastException($"Bad id {dboId} does not refer to a {type.ToLower()}");
            }
        }
    }

    protected virtual void Delta(T before, T after, List<string> patches)
    {
    }

    protected abstract IEnumerable<FullyQualifiedName> Selector(IDatabase db);
}
