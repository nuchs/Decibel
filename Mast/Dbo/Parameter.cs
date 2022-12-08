using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class Parameter
{
    public Parameter(ProcedureParameter parameter)
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

    public string Name { get; }

    public override string ToString()
    {
        var nullSpec = IsNullable is null ? string.Empty :
                       IsNullable.Value ? " NULL" : " NOT NULL";
        var defaultSpec = string.IsNullOrWhiteSpace(Default) ? string.Empty : $" = {Default}";
        return $"@{Name} {DataType}{nullSpec}{defaultSpec}";
    }

    private static string AssembleDefaultValue(ProcedureParameter parameter)
    {
        if (parameter.Value is not null)
        {
            var dfltTokens = parameter.ScriptTokenStream
                    .Take(parameter.Value.FirstTokenIndex..(parameter.Value.LastTokenIndex + 1))
                    .Select(d => d.Text);
            return string.Join("", dfltTokens).Trim();
        }

        return string.Empty;
    }

    private static string GetName(ProcedureParameter parameter)
    {
        return parameter.VariableName.Value;
    }

    private static bool? GetNullability(ProcedureParameter parameter)
    {
        return parameter.Nullable is null ? null : parameter.Nullable.Nullable;
    }

    private ParameterMod GetModifier(ProcedureParameter parameter)
                                            => parameter.Modifier switch
                                            {
                                                ParameterModifier.None => ParameterMod.None,
                                                ParameterModifier.Output => ParameterMod.Output,
                                                ParameterModifier.ReadOnly => ParameterMod.Readonly,
                                                _ => throw new InvalidDataException($"Unrecognised parameter modifier {parameter.Modifier}")
                                            };
}
