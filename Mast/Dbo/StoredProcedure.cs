using Mast.Parsing;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class StoredProcedure : DbObject
{
    public StoredProcedure(CreateProcedureStatement node)
        : base(node)
    {
        Identifier = AssembleIdentifier(node);
        Parameters = CollectParameters(node);
    }

    public List<Parameter> Parameters { get; }

    private protected override (IEnumerable<DbObject>, IEnumerable<FullyQualifiedName>) GetReferents(Database db)
    {
        var (schemaHits, schmeaMisses) = CorralateRefs(db.Schemas, FullyQualifiedName.FromSchema(Identifier.Schema));
        var (typeHits, typeMisses) = CorralateRefs(db.ScalarTypes.OfType<DbObject>().Concat(db.TableTypes), Parameters.Select(c => c.DataType));

        return (schemaHits.Concat(typeHits), schmeaMisses.Concat(typeMisses));
    }

    private FullyQualifiedName AssembleIdentifier(CreateProcedureStatement node)
        => FullyQualifiedName.FromSchemaName(GetId(node.ProcedureReference.Name.SchemaIdentifier), GetId(node.ProcedureReference.Name.BaseIdentifier));

    private List<Parameter> CollectParameters(CreateProcedureStatement node)
        => node.Parameters.Select(p => new Parameter(p)).ToList();
}
