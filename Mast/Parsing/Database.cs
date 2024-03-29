﻿using Mast.Dbo;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

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
    private readonly HashSet<Reference> unresolvedReferencesSet = new();
    private readonly List<User> userList = new();
    private readonly List<View> viewList = new();

    public int Count => nameMap.Count;

    public IEnumerable<Function> Functions => functionList;

    public IEnumerable<FullyQualifiedName> Keys => nameMap.Keys;

    public IEnumerable<StoredProcedure> Procedures => procedureList;

    public IEnumerable<ScalarType> ScalarTypes => scalarTypeList;

    public IEnumerable<Schema> Schemas => schemaList;

    public IEnumerable<Table> Tables => tableList;

    public IEnumerable<TableType> TableTypes => tableTypeList;

    public IEnumerable<Trigger> Triggers => triggerList;

    public IEnumerable<Reference> UnresolvedReferences => unresolvedReferencesSet;

    public IEnumerable<User> Users => userList;

    public IEnumerable<DbObject> Values => nameMap.Values;

    public IEnumerable<View> Views => viewList;

    public DbObject this[FullyQualifiedName key]
        => nameMap[key];

    public bool ContainsKey(FullyQualifiedName key)
        => nameMap.ContainsKey(key);

    public IEnumerator<KeyValuePair<FullyQualifiedName, DbObject>> GetEnumerator()
        => nameMap.GetEnumerator();

    public bool TryGetValue(FullyQualifiedName key, [MaybeNullWhen(false)] out DbObject value)
        => nameMap.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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

    internal void ResolveReference(DbObject referee, FullyQualifiedName candidate)
    {
        if (IdentifierBlackList.Contains(candidate))
        {
            return;
        }

        if (TryGetValue(candidate, out var referent))
        {
            referent.Referees.Add(referee);
        }
        else
        {
            unresolvedReferencesSet.Add(new Reference(referee, candidate));
        }
    }
}
