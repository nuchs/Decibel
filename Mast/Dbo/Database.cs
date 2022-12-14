namespace Mast.Dbo;

public class Database
{
    public List<Function> Functions { get; set; } = new List<Function>();

    public List<StoredProcedure> Procedures { get; set; } = new List<StoredProcedure>();

    public List<ScalarType> ScalarTypes { get; set; } = new List<ScalarType>();

    public List<Table> Tables { get; set; } = new List<Table>();

    public List<TableType> TableTypes { get; set; } = new List<TableType>();
}
