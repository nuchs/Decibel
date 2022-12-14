using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class DefaultConstraint : DbObject
{
    public DefaultConstraint(DefaultConstraintDefinition def)
        : base(def)
    {
        Name = GetName(def);
        Value = GetValue(def);
    }

    public string Value { get; }

    private static string GetName(DefaultConstraintDefinition def)
       => def.ConstraintIdentifier?.Value ?? string.Empty;

    private string GetValue(DefaultConstraintDefinition def)
                => AssembleFragment(def, def.FirstTokenIndex + 1, def.LastTokenIndex + 1);
}
