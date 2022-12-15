using Mast.Parsing;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class IdentityConstraint : DbObject
{
    public IdentityConstraint(IdentityOptions id)
        : base(id)
    {
        Seed = AssembleIdentitySeed(id);
        Increment = AssembleIdentityIncrement(id);
    }

    public int Increment { get; }

    public int Seed { get; }

    internal override void CrossReference(Database db) => throw new NotImplementedException();

    private int AssembleIdentityIncrement(IdentityOptions id)
    {
        if (id.IdentityIncrement is null)
        {
            return 1;
        }

        return int.Parse(AssembleFragment(id.IdentityIncrement));
    }

    private int AssembleIdentitySeed(IdentityOptions id)
    {
        if (id.IdentitySeed is null)
        {
            return 1;
        }

        return int.Parse(AssembleFragment(id.IdentitySeed));
    }
}
