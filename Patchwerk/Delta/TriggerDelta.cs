using Mast;
using Mast.Dbo;

namespace Patchwerk.Delta;

internal sealed class TriggerDelta : DboDelta<Trigger>
{
    public TriggerDelta()
        : base("Trigger")
    {
    }

    protected override void Delta(Trigger before, Trigger after, List<string> patches)
    {
        patches.Add($"DROP TRIGGER {before.Identifier}");
        patches.Add(after.Content);
    }

    protected override IEnumerable<FullyQualifiedName> Selector(IDatabase db)
        => db.Triggers.Select(u => u.Identifier);
}
