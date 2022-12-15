using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class Parameter : DbFragment
{
    public Parameter(ProcedureParameter parameter)
        : base(parameter)
    {
        Name = GetName(parameter);
        IsNullable = GetNullability(parameter);
        DataType = GetTypeId(parameter.DataType);
        Default = AssembleDefaultValue(parameter);
        Modifier = GetModifier(parameter);
    }

    public FullyQualifiedName DataType { get; }

    public string Default { get; }

    public bool? IsNullable { get; }

    public ParameterMod Modifier { get; }

    public string Name { get; }

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

    private string GetName(ProcedureParameter parameter) => GetId(parameter.VariableName);

    private FullyQualifiedName GetTypeId(DataTypeReference dataType)
        => new(GetId(dataType.Name.SchemaIdentifier), GetId(dataType.Name.BaseIdentifier));
}
