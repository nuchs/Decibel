using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class DefaultConstraint : DbFragment
{
    public DefaultConstraint(DefaultConstraintDefinition def)
        : base(def)
    {
        Name = GetName(def);
        Value = GetValue(def);
    }

    public string Name { get; }

    public string Value { get; }

    private string GetName(DefaultConstraintDefinition def)
       => GetId(def.ConstraintIdentifier);

    private string GetValue(DefaultConstraintDefinition def)
       => AssembleFragment(def, def.FirstTokenIndex + 1, def.LastTokenIndex + 1);
}
