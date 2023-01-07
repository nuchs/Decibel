using Mast;
using Mast.Dbo;

namespace Patchwerk.Delta;

internal class SchemaDelta : DbDelta<Schema>
{
    public SchemaDelta()
        : base("Schema")
    {
    }

    protected override IEnumerable<FullyQualifiedName> Selector(IDatabase db)
        => db.Schemas.Select(u => u.Identifier);
}
