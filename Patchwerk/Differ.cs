using Mast;
using Patchwerk.Delta;

namespace Patchwerk;

public sealed class Differ
{
    public string GeneratePatches(IDatabase before, IDatabase after)
    {
        List<string> patches = new();

        new UserDelta().GeneratePatches(before, after, patches);
        new SchemaDelta().GeneratePatches(before, after, patches);
        new TriggerDelta().GeneratePatches(before, after, patches);

        return string.Join("\nGO\n\n", patches);
    }
}
