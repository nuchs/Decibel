using Mast;
using Mast.Dbo;

namespace Patchwerk.Delta;

internal sealed class ViewDelta : DboDelta<View>
{
    public ViewDelta()
        : base("View")
    {
    }

    internal override IEnumerable<string> GenerateAddsAndUpdates(IDatabase before, IDatabase after)
    {
        return base.GenerateAddsAndUpdates(before, after);
    }

    protected override IEnumerable<DbObject> Selector(IDatabase db)
        => db.Views;
}
