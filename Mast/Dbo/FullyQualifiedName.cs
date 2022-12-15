namespace Mast.Dbo;

public sealed record FullyQualifiedName(string Schema, string Name)
{
    public override string ToString()
    {
        var separator = string.IsNullOrWhiteSpace(Schema) ? string.Empty : ".";

        return $"{Schema.Trim()}{separator}{Name.Trim()}";
    }
}
