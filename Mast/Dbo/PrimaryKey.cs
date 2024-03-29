﻿using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class PrimaryKey : DbFragment
{
    public PrimaryKey(Column column, UniqueConstraintDefinition constraint)
        : this(new[] { column }, constraint)
    {
    }

    public PrimaryKey(IEnumerable<Column> columns, UniqueConstraintDefinition constraint)
        : base(constraint)
    {
        if (!constraint.IsPrimaryKey)
        {
            throw new InvalidOperationException("Cannot create a primary key from a non-primary unqiue constraint");
        }

        Name = GetName(constraint);
        Columns = CollectPrimaryColumns(columns, constraint);
        Clustered = constraint.Clustered ?? false;
    }

    public bool Clustered { get; }

    public IEnumerable<Column> Columns { get; }

    public string Name { get; }

    private IEnumerable<Column> CollectPrimaryColumns(IEnumerable<Column> columns, UniqueConstraintDefinition constraint)
    {
        if (columns.Count() == 1)
        {
            return columns;
        }

        var primaryColIds = constraint.Columns.Select(c => c.Column.MultiPartIdentifier.Identifiers);
        var primaryColNames = primaryColIds.Select(p => string.Join(".", p.Select(i => GetId(i))));

        return columns.Where(c => primaryColNames.Contains(c.Name));
    }

    private string GetName(UniqueConstraintDefinition constraint)
    {
        return GetId(constraint.ConstraintIdentifier);
    }
}
