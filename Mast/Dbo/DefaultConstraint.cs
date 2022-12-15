using Mast.Parsing;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class DefaultConstraint : DbObject
{
    public DefaultConstraint(DefaultConstraintDefinition def)
        : base(def)
    {
        Name = GetName(def);
        Value = GetValue(def);
    }

    public string Value { get; }

    internal override void CrossReference(Database db) => throw new NotImplementedException();
    private string GetName(DefaultConstraintDefinition def)
       => GetId(def.ConstraintIdentifier);

    private string GetValue(DefaultConstraintDefinition def)
       => AssembleFragment(def, def.FirstTokenIndex + 1, def.LastTokenIndex + 1);
}
