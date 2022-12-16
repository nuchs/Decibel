using Mast.Parsing;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Linq;

namespace Mast.Dbo;

public sealed class Function : DbObject
{
    public Function(CreateFunctionStatement node)
        : base(node)
    {
        Identifier = AssembleIdentifier(node);
        Parameters = CollectParameters(node);
        ReturnType = AssembleReturnType(node);
    }

    public IEnumerable<Parameter> Parameters { get; }

    public string ReturnType { get; }

    private protected override (IEnumerable<DbObject>, IEnumerable<FullyQualifiedName>) GetReferents(Database db)
    {
        var (schemaHits, schmeaMisses) = CorralateRefs(db.Schemas, FullyQualifiedName.FromSchema(Identifier.Schema));
        var (typeHits, typeMisses) = CorralateRefs(db.ScalarTypes.OfType<DbObject>().Concat(db.TableTypes), Parameters.Select(c => c.DataType));

        return (schemaHits.Concat(typeHits), schmeaMisses.Concat(typeMisses));
    }

    private FullyQualifiedName AssembleIdentifier(CreateFunctionStatement node)
        => FullyQualifiedName.FromSchemaName(GetId(node.Name.SchemaIdentifier), GetId(node.Name.BaseIdentifier));

    private string AssembleReturnType(CreateFunctionStatement node)
        => AssembleFragment(node.ReturnType);

    private List<Parameter> CollectParameters(CreateFunctionStatement node)
        => node.Parameters.Select(p => new Parameter(p)).ToList();
}
