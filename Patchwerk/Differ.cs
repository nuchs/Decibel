using Mast;
using Patchwerk.Delta;

namespace Patchwerk;

public sealed class Differ
{
    private readonly SchemaDelta schema = new();
    private readonly UserDelta user = new();
    private readonly TriggerDelta trigger = new();

    public string GeneratePatches(IDatabase before, IDatabase after)
    {
        List<string> patches = new();

        patches.AddRange(schema.GenerateAddsAndUpdates(before, after));
        patches.AddRange(user.GenerateAddsAndUpdates(before, after));
        patches.AddRange(trigger.GenerateAddsAndUpdates(before, after));

        patches.AddRange(trigger.GenerateDrops(before, after));
        patches.AddRange(user.GenerateDrops(before, after));
        patches.AddRange(schema.GenerateDrops(before, after));

        return string.Join("\nGO\n\n", patches);
    }
}
