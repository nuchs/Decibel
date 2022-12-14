using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class Trigger : DbObject
{
    public Trigger(CreateTriggerStatement trigger)
        : base(trigger)
    {
    }
}
