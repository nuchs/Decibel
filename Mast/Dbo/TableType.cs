﻿using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public class TableType
{
    public string Content;
    public List<object> ReferencedBy = new();
    public List<object> References = new();
    public string Name;
    public string Schema;
    public List<Column> Columns = new();
    public List<Index> Indices = new();
    public PrimaryKey Primary;

    public TableType(CreateTypeTableStatement node)
    {
        var tokenValues = node.ScriptTokenStream.Select(t => t.Text);
        Content = string.Join(string.Empty, tokenValues);

        Schema = node.Name.SchemaIdentifier.Value;

        Name = node.Name.BaseIdentifier.Value;

        foreach (var colDef in node.Definition.ColumnDefinitions)
        {
            Column column = new Column(colDef);
            Columns.Add(column);
        }

        foreach (var index in node.Definition.Indexes)
        {
            Indices.Add(new Index(Columns, index));
        }

        var primaryCol = node.Definition.ColumnDefinitions.FirstOrDefault(p => p.Constraints.OfType<UniqueConstraintDefinition>().Where(uq => uq.IsPrimaryKey).Any());
        var compoundPrimary = node.Definition.TableConstraints.OfType<UniqueConstraintDefinition>().FirstOrDefault(uq => uq.IsPrimaryKey);
        Primary = primaryCol != null ? new PrimaryKey(primaryCol) :
             compoundPrimary != null ? new PrimaryKey(compoundPrimary) : null;
    }
  }