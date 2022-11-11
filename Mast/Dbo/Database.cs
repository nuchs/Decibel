using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mast.Dbo;
public class Database
{
    public List<StoredProcedure> Procedures { get; set; } = new List<StoredProcedure>();
    public List<Function> Functions { get; set; } = new List<Function>();
    public List<TableType> Types { get; set; } = new List<TableType>();
    public List<Table> Tables { get; set; } = new List<Table>();


}
