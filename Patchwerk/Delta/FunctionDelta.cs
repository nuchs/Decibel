using Mast;
using Mast.Dbo;

namespace Patchwerk.Delta;

internal sealed class FunctionDelta : DboDelta<Function>
{
    public FunctionDelta()
        : base("Function")
    {
    }

    protected override IEnumerable<DbObject> Selector(IDatabase db)
        => db.Functions;
}
