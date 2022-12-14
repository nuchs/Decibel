using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mast.Dbo;
public class Schema : DbObject
{
    public Schema(CreateSchemaStatement schema)
        : base(schema)
    {

    }
}
