using Mast.Dbo;

namespace Mast.Parsing;

internal sealed class Database : IDatabase
{
    private readonly List<Function> functionList = new();
    private readonly Dictionary<FullyQualifiedName, DbObject> nameMap = new();
    private readonly List<StoredProcedure> procedureList = new();
    private readonly List<ScalarType> scalarTypeList = new();
    private readonly List<Schema> schemaList = new();
    private readonly List<Table> tableList = new();
    private readonly List<TableType> tableTypeList = new();
    private readonly List<Trigger> triggerList = new();
    private readonly List<User> userList = new();
    private readonly HashSet<Reference> unresolvedReferencesSet = new();
    private readonly List<View> viewList = new();
    private readonly HashSet<FullyQualifiedName> blacklist = new()
    {
        FullyQualifiedName.FromName("TYPE"),
        FullyQualifiedName.FromName("bit"),
        FullyQualifiedName.FromName("tinyint"),
        FullyQualifiedName.FromName("smallint"),
        FullyQualifiedName.FromName("int"),
        FullyQualifiedName.FromName("bigint"),
        FullyQualifiedName.FromName("decimal"),
        FullyQualifiedName.FromName("numeric"),
        FullyQualifiedName.FromName("money"),
        FullyQualifiedName.FromName("smallmoney"),
        FullyQualifiedName.FromName("float"),
        FullyQualifiedName.FromName("real"),
        FullyQualifiedName.FromName("date"),
        FullyQualifiedName.FromName("time"),
        FullyQualifiedName.FromName("datetime"),
        FullyQualifiedName.FromName("datetime2"),
        FullyQualifiedName.FromName("datetimeoffset"),
        FullyQualifiedName.FromName("smalldatetime"),
        FullyQualifiedName.FromName("char"),
        FullyQualifiedName.FromName("varchar"),
        FullyQualifiedName.FromName("text"),
        FullyQualifiedName.FromName("binary"),
        FullyQualifiedName.FromName("varbinary"),
        FullyQualifiedName.FromName("image"),
        FullyQualifiedName.FromName("hierarchyid"),
        FullyQualifiedName.FromName("sql_varient"),
        FullyQualifiedName.FromName("geometry"),
        FullyQualifiedName.FromName("rowversion"),
        FullyQualifiedName.FromName("uniqueidentifier"),
        FullyQualifiedName.FromName("xml"),
    };

    public IEnumerable<Function> Functions => functionList;
    public IEnumerable<StoredProcedure> Procedures => procedureList;
    public IEnumerable<ScalarType> ScalarTypes => scalarTypeList;
    public IEnumerable<Schema> Schemas => schemaList;
    public IEnumerable<Table> Tables => tableList;
    public IEnumerable<TableType> TableTypes => tableTypeList;
    public IEnumerable<Trigger> Triggers => triggerList;
    public IEnumerable<Reference> UnresolvedReferences => unresolvedReferencesSet;
    public IEnumerable<User> Users => userList;
    public IEnumerable<View> Views => viewList;

    internal IReadOnlyDictionary<FullyQualifiedName, DbObject> NameMap => nameMap;

    internal void AddObject(DbObject dbObject)
    {
        nameMap.Add(dbObject.Identifier, dbObject);

        switch (dbObject)
        {
            case Function obj:
                functionList.Add(obj);
                break;

            case StoredProcedure obj:
                procedureList.Add(obj);
                break;

            case ScalarType obj:
                scalarTypeList.Add(obj);
                break;

            case Schema obj:
                schemaList.Add(obj);
                break;

            case Table obj:
                tableList.Add(obj);
                break;

            case TableType obj:
                tableTypeList.Add(obj);
                break;

            case Trigger obj:
                triggerList.Add(obj);
                break;

            case User obj:
                userList.Add(obj);
                break;

            case View obj:
                viewList.Add(obj);
                break;

            default:
                throw new InvalidDataException($"Unrecognised type of DbObject : {dbObject.GetType()}");
        }
    }

    internal void AddUnresolvedRefs(DbObject referee, IEnumerable<FullyQualifiedName> unresolvedRefs)
        => unresolvedReferencesSet.UnionWith(
            unresolvedRefs
            .Where(u => !blacklist.Contains(u))
            .Select(u => new Reference(referee, u))); 
}
