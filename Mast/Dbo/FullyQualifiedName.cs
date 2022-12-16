using System.Text;

namespace Mast.Dbo;

public sealed record FullyQualifiedName(string Db, string Schema, string Name)
{
    public static FullyQualifiedName None { get; } = new(string.Empty, string.Empty, string.Empty);

    public static FullyQualifiedName FromDb(string db) => new(db, string.Empty, string.Empty);

    public static FullyQualifiedName FromName(string name) => new(string.Empty, string.Empty, name);

    public static FullyQualifiedName FromSchema(string schema) => new(string.Empty, schema, string.Empty);

    public static FullyQualifiedName FromSchemaName(string schema, string name) => new(string.Empty, schema, name);

    public override string ToString() => string.Join('.', Db, Schema, Name).Trim('.');
}
