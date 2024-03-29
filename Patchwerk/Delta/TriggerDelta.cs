﻿using Mast;
using Mast.Dbo;

namespace Patchwerk.Delta;

internal sealed class TriggerDelta : DboDelta<Trigger>
{
    public TriggerDelta()
        : base("Trigger")
    {
    }

    protected override IEnumerable<DbObject> Selector(IDatabase db)
        => db.Triggers;
}
