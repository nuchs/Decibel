using Mast;
using Patchwerk.Delta;

namespace Patchwerk;

public sealed class Differ
{
    private readonly FunctionDelta function = new();
    private readonly ProcedureDelta procedure = new();
    private readonly SchemaDelta schema = new();
    private readonly TriggerDelta trigger = new();
    private readonly UserDelta user = new();
    private readonly ViewDelta view = new();
    private readonly TableDelta table = new();
    private readonly TypeDelta type = new();

    public string GeneratePatches(IDatabase before, IDatabase after)
    {
        List<string> patches = new();

        patches.AddRange(schema.GenerateAddsAndUpdates(before, after));
        patches.AddRange(user.GenerateAddsAndUpdates(before, after));
        patches.AddRange(type.GenerateAddsAndUpdates(before, after));
        patches.AddRange(table.GenerateAddsAndUpdates(before, after));
        patches.AddRange(view.GenerateAddsAndUpdates(before, after));
        patches.AddRange(function.GenerateAddsAndUpdates(before, after));
        patches.AddRange(procedure.GenerateAddsAndUpdates(before, after));
        patches.AddRange(trigger.GenerateAddsAndUpdates(before, after));

        patches.AddRange(trigger.GenerateDrops(before, after));
        patches.AddRange(procedure.GenerateDrops(before, after));
        patches.AddRange(function.GenerateDrops(before, after));
        patches.AddRange(view.GenerateDrops(before, after));
        patches.AddRange(table.GenerateDrops(before, after));
        patches.AddRange(type.GenerateDrops(before, after));
        patches.AddRange(user.GenerateDrops(before, after));
        patches.AddRange(schema.GenerateDrops(before, after));

        return string.Join("\nGO\n\n", patches);
    }
}
