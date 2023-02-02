using Mast;
using Mast.Dbo;

namespace Patchwerk.Delta;

internal sealed class TriggerDelta : DboDelta<Trigger>
{
    public TriggerDelta()
        : base("Trigger")
    {
    }

    protected override string Delta(Trigger pre, Trigger post)
        => $"""
        DROP TRIGGER {pre.Identifier}
        GO

        {post.Content}
        """;

    protected override IEnumerable<FullyQualifiedName> Selector(IDatabase db)
        => db.Triggers.Select(u => u.Identifier);
}
