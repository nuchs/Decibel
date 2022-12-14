using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class View : DbObject
{
    public View(CreateViewStatement view)
        : base(view)
    {
    }
}
