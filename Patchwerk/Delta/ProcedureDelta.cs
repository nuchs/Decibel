using Mast;
using Mast.Dbo;

namespace Patchwerk.Delta;

internal sealed class ProcedureDelta : DboDelta<StoredProcedure>
{
    public ProcedureDelta()
        : base("Procedure")
    {
    }

    protected override IEnumerable<DbObject> Selector(IDatabase db)
        => db.Procedures;
}
