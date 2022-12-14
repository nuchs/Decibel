namespace Mast.Dbo;

public class Database : IDatabase
{
    public IEnumerable<Function> Functions => FunctionList;
    public IEnumerable<StoredProcedure> Procedures => ProcedureList;
    public IEnumerable<ScalarType> ScalarTypes => ScalarTypeList;
    public IEnumerable<Schema> Schemas => SchemaList;
    public IEnumerable<Table> Tables => TableList;
    public IEnumerable<TableType> TableTypes => TableTypeList;
    public IEnumerable<Trigger> Triggers => TriggerList;
    public IEnumerable<User> Users => UserList;
    public IEnumerable<View> Views => ViewList;

    internal List<Function> FunctionList { get; } = new List<Function>();

    internal List<StoredProcedure> ProcedureList { get; } = new List<StoredProcedure>();

    internal List<ScalarType> ScalarTypeList { get; } = new List<ScalarType>();

    internal List<Schema> SchemaList { get; } = new List<Schema>();

    internal List<Table> TableList { get; } = new List<Table>();

    internal List<TableType> TableTypeList { get; } = new List<TableType>();

    internal List<Trigger> TriggerList { get; } = new List<Trigger>();

    internal List<User> UserList { get; } = new List<User>();

    internal List<View> ViewList { get; } = new List<View>();
}
