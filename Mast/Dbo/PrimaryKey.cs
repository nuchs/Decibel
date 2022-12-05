using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mast.Dbo;
public class PrimaryKey
{
    public string Name;
    public IEnumerable<Column> Columns;

    public PrimaryKey(IEnumerable<Column> columns, ColumnDefinition primaryCol)
    {
    }

    public PrimaryKey(IEnumerable<Column> columns, UniqueConstraintDefinition compoundPrimary)
    {
    }
}
