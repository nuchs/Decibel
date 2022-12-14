using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class IdentityConstraint : DbObject
{
	public IdentityConstraint(IdentityOptions id)
		: base(id)
	{
        Seed = AssembleIdentitySeed(id);
        Increment = AssembleIdentityIncrement(id);
	}

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

    public int Seed { get; }
	
	public int Increment { get; }

}
