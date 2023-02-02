using Mast;
using Mast.Dbo;

namespace Patchwerk.Delta;

internal sealed class SchemaDelta : DboDelta<Schema>
{
    public SchemaDelta()
        : base("Schema")
    {
    }

    protected override string Delta(Schema pre, Schema post) 
        => throw new InvalidOperationException($"Schemas cannot be modified : {pre.Identifier}");

    protected override IEnumerable<DbObject> Selector(IDatabase db)
        => db.Schemas;
}
