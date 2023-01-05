namespace Mast.Dbo;

internal static class IdentifierBlackList
{
    private static readonly HashSet<FullyQualifiedName> items = new()
    {
        FullyQualifiedName.FromName("type"),
        FullyQualifiedName.FromName("bit"),
        FullyQualifiedName.FromName("tinyint"),
        FullyQualifiedName.FromName("smallint"),
        FullyQualifiedName.FromName("int"),
        FullyQualifiedName.FromName("bigint"),
        FullyQualifiedName.FromName("decimal"),
        FullyQualifiedName.FromName("numeric"),
        FullyQualifiedName.FromName("money"),
        FullyQualifiedName.FromName("smallmoney"),
        FullyQualifiedName.FromName("float"),
        FullyQualifiedName.FromName("real"),
        FullyQualifiedName.FromName("date"),
        FullyQualifiedName.FromName("time"),
        FullyQualifiedName.FromName("datetime"),
        FullyQualifiedName.FromName("datetime2"),
        FullyQualifiedName.FromName("datetimeoffset"),
        FullyQualifiedName.FromName("smalldatetime"),
        FullyQualifiedName.FromName("char"),
        FullyQualifiedName.FromName("varchar"),
        FullyQualifiedName.FromName("text"),
        FullyQualifiedName.FromName("binary"),
        FullyQualifiedName.FromName("varbinary"),
        FullyQualifiedName.FromName("image"),
        FullyQualifiedName.FromName("hierarchyid"),
        FullyQualifiedName.FromName("sql_varient"),
        FullyQualifiedName.FromName("geometry"),
        FullyQualifiedName.FromName("rowversion"),
        FullyQualifiedName.FromName("uniqueidentifier"),
        FullyQualifiedName.FromName("xml"),
        FullyQualifiedName.FromName("after"),
        FullyQualifiedName.FromName("before"),
        FullyQualifiedName.FromName("returns"),
        FullyQualifiedName.FromName("readonly"),
    };

    internal static bool Contains(FullyQualifiedName name)
        => items.Contains(name);

    internal static bool Contains(string name)
        => items.Contains(FullyQualifiedName.FromName(name));
}
