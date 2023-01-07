using Mast;
using Mast.Dbo;

namespace Patchwerk.Delta;

internal sealed class UserDelta : DboDelta<User>
{
    public UserDelta()
        : base("User")
    {
    }

    protected override void Delta(User before, User after, List<string> patches)
    {
        var schemaPart = before.DefaultSchema == after.DefaultSchema ? "" : $"DEFAULT_SCHEMA = {after.DefaultSchema}";
        var loginPart = before.Login == after.Login ? "" : $"LOGIN = {after.Login}";
        var languagePart = before.DefaultLanguage == after.DefaultLanguage ? "" : $"DEFAULT_LANGUAGE = {after.DefaultLanguage}";

        var mods = string.Join(", ", new string[] { schemaPart, loginPart, languagePart, }.Where(s => !string.IsNullOrWhiteSpace(s)));

        patches.Add($"ALTER USER {after.Identifier} WITH {mods}");
    }

    protected override IEnumerable<FullyQualifiedName> Selector(IDatabase db)
        => db.Users.Select(u => u.Identifier);
}
