using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class Parameter : DbObject
{
    public Parameter(ProcedureParameter parameter)
        : base(parameter)
    {
        Name = GetName(parameter);
        IsNullable = GetNullability(parameter);
        DataType = new ScalarType(parameter.DataType);
        Default = AssembleDefaultValue(parameter);
        Modifier = GetModifier(parameter);
    }

    public ScalarType DataType { get; }

    public string Default { get; }

    public bool? IsNullable { get; }

    public ParameterMod Modifier { get; }

    private string GetName(ProcedureParameter parameter) => GetId(parameter.VariableName);

    private static bool? GetNullability(ProcedureParameter parameter)
        => parameter.Nullable is null ? null : parameter.Nullable.Nullable;

    private string AssembleDefaultValue(ProcedureParameter parameter)
        => parameter.Value is not null ? AssembleFragment(parameter.Value) : string.Empty;

    private ParameterMod GetModifier(ProcedureParameter parameter)
        => parameter.Modifier switch
        {
            ParameterModifier.None => ParameterMod.None,
            ParameterModifier.Output => ParameterMod.Output,
            ParameterModifier.ReadOnly => ParameterMod.Readonly,
            _ => throw new InvalidDataException($"Unrecognised parameter modifier {parameter.Modifier}")
        };
}
