using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class ForeignKey : DbFragment
{
    public ForeignKey(
        IEnumerable<Column> columns,
        ForeignKeyConstraintDefinition constraint)
        : base(constraint)
    {
        Name = GetName(constraint);
        Column = GetColumn(columns, constraint);
        (ForeignTable, ForeignColumn) = GetForeignReference(constraint);
        OnDelete = GetDeleteAction(constraint);
        OnUpdate = GetUpdateAction(constraint);
    }

    public ForeignKey(Column column, ForeignKeyConstraintDefinition constraint)
        : this(new[] { column }, constraint)
    {
    }

    public Column Column { get; }

    public CaseInsensitiveString ForeignColumn { get; }

    public FullyQualifiedName ForeignTable { get; }

    public string Name { get; }

    public ChangeAction OnDelete { get; }

    public ChangeAction OnUpdate { get; }

    private Column GetColumn(IEnumerable<Column> columns, ForeignKeyConstraintDefinition constraint)
    {
        if (constraint.Columns.Count > 1)
        {
            throw new NotSupportedException("Mutli-column foreign keys not supported yet");
        }

        if (columns.Count() == 1)
        {
            return columns.First();
        }

        var id = GetId(constraint.Columns.First());

        return columns.Where(c => c.Name == id).First();
    }

    private ChangeAction GetDeleteAction(ForeignKeyConstraintDefinition constraint)
        => MapAction(constraint.DeleteAction);

    private (FullyQualifiedName table, string column) GetForeignReference(ForeignKeyConstraintDefinition constraint)
    {
        if (constraint.ReferencedTableColumns.Count > 1)
        {
            throw new NotSupportedException("Mutli-column foreign keys not supported yet");
        }

        return (
            FullyQualifiedName.FromSchemaName(
                GetId(constraint.ReferenceTableName.SchemaIdentifier), 
                GetId(constraint.ReferenceTableName.BaseIdentifier)),
            GetId(constraint.ReferencedTableColumns.First()));
    }

    private string GetName(ForeignKeyConstraintDefinition constraint)
        => GetId(constraint.ConstraintIdentifier);

    private ChangeAction GetUpdateAction(ForeignKeyConstraintDefinition constraint)
        => MapAction(constraint.UpdateAction);

    private ChangeAction MapAction(DeleteUpdateAction deleteAction)
        => deleteAction switch
        {
            DeleteUpdateAction.NotSpecified => ChangeAction.NotSet,
            DeleteUpdateAction.NoAction => ChangeAction.NoAction,
            DeleteUpdateAction.Cascade => ChangeAction.Cascade,
            DeleteUpdateAction.SetDefault => ChangeAction.SetDefault,
            DeleteUpdateAction.SetNull => ChangeAction.SetNull,
            _ => throw new InvalidDataException($"Unrecognised delete action - {deleteAction}")
        };
}
