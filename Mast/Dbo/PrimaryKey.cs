using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class PrimaryKey
{
    private readonly List<Column> columns = new();

    public PrimaryKey(Column column, UniqueConstraintDefinition constraint)
        : this(new[] { column }, constraint)
    {
    }

    public PrimaryKey(IEnumerable<Column> columns, UniqueConstraintDefinition constraint)
    {
        if (!constraint.IsPrimaryKey)
        {
            throw new InvalidOperationException("Cannot create a primary key from a non-primary unqiue constraint");
        }

        this.columns.AddRange(columns);
        Name = constraint.ConstraintIdentifier?.Value ?? string.Empty;
        Clustered = constraint.Clustered ?? false;
        Content = AssemblePrimaryKeyContent(constraint);
    }

    private static string AssemblePrimaryKeyContent(UniqueConstraintDefinition constraint)
    {
        var tokenValues = constraint
           .ScriptTokenStream
           .Take(constraint.FirstTokenIndex..(constraint.LastTokenIndex + 1))
           .Select(t => t.Text);

        return string.Join(string.Empty, tokenValues);
    }

    public bool Clustered { get; }

    public IEnumerable<Column> Columns => columns;

    public string Name { get; }

    public string Content { get; }

    public override string ToString() => Content;
}
