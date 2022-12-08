using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class Column
{
    public string name;
    public ScalarType DataType;

    public bool IsNullable;
    public bool IsPrimary;
    public string Default = String.Empty;
    public string DefaultName = String.Empty;

    public List<object> referencedBy = new();

    public Column(ColumnDefinition colDef)
    {
        name = colDef.ColumnIdentifier.Value;
        DataType = new ScalarType(colDef.DataType);

        IsNullable = colDef.Constraints.OfType<NullableConstraintDefinition>().Select(n => n.Nullable).FirstOrDefault();
        if (colDef.DefaultConstraint is not null)
        {
            var dfltTokens = colDef.DefaultConstraint.ScriptTokenStream
        .Take(colDef.DefaultConstraint.FirstTokenIndex..(colDef.DefaultConstraint.LastTokenIndex + 1))
        .Select(d => d.Text);
            Default = string.Join("", dfltTokens).Trim();
            DefaultName = colDef.DefaultConstraint.ConstraintIdentifier?.Value ?? String.Empty;
        }

    }
}
