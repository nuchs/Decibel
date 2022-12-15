using Mast.Dbo;

namespace Mast.Parsing;

internal sealed class Database : IDatabase
{
    public IEnumerable<Function> Functions => FunctionList;

    public IEnumerable<StoredProcedure> Procedures => ProcedureList;

    public IEnumerable<ScalarType> ScalarTypes => ScalarTypeList;

    public IEnumerable<Schema> Schemas => SchemaList;

    public IEnumerable<Table> Tables => TableList;

    public IEnumerable<TableType> TableTypes => TableTypeList;

    public IEnumerable<Trigger> Triggers => TriggerList;

    public IEnumerable<Reference> UnresolvedReferences => UnresolvedReferencesList;

    public IEnumerable<User> Users => UserList;

    public IEnumerable<View> Views => ViewList;

    internal List<Function> FunctionList { get; } = new();

    internal List<StoredProcedure> ProcedureList { get; } = new();

    internal List<ScalarType> ScalarTypeList { get; } = new();

    internal List<Schema> SchemaList { get; } = new();

    internal List<Table> TableList { get; } = new();

    internal List<TableType> TableTypeList { get; } = new();

    internal List<Trigger> TriggerList { get; } = new();

    internal List<Reference> UnresolvedReferencesList { get; } = new();

    internal List<User> UserList { get; } = new();

    internal List<View> ViewList { get; } = new();
}
