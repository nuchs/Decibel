namespace Mast.Dbo;

public sealed class FullyQualifiedName
{
    public FullyQualifiedName(string db, string schema, string name)
    {
        Db = db.Trim(new char[] { '[', ']' });
        Schema = schema.Trim(new char[] { '[', ']' });
        Name = name.Trim(new char[] { '[', ']' });
    }

    public static FullyQualifiedName None { get; } = new(string.Empty, string.Empty, string.Empty);

    public CaseInsensitiveString Db { get; }
    public CaseInsensitiveString Name { get; }
    public CaseInsensitiveString Schema { get; }

    public static FullyQualifiedName FromDb(string db) => new(db, string.Empty, string.Empty);

    public static FullyQualifiedName FromDbSchema(string db, string schema) => new(db, schema, string.Empty);

    public static FullyQualifiedName FromName(string name) => new(string.Empty, string.Empty, name);

    public static FullyQualifiedName FromSchema(string schema) => new(string.Empty, schema, string.Empty);

    public static FullyQualifiedName FromSchemaName(string schema, string name) => new(string.Empty, schema, name);

    public static bool operator !=(FullyQualifiedName left, FullyQualifiedName right)
        => !(left == right);

    public static bool operator ==(FullyQualifiedName left, FullyQualifiedName right)
      => left.Equals(right);

    public override bool Equals(object? obj) =>
        obj is FullyQualifiedName other &&
        Db.Equals(other.Db) &&
        Schema.Equals(other.Schema) &&
        Name.Equals(other.Name);

    public override int GetHashCode() => HashCode.Combine(Db, Schema, Name);

    public override string ToString() => string.Join('.', Db, Schema, Name).Trim('.');
}
