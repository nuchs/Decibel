using Mast;
using Mast.Dbo;

namespace Patchwerk.Delta;

internal sealed class UserDelta : DboDelta<User>
{
    public UserDelta()
        : base("User")
    {
    }

    protected override string Delta(User pre, User post)
    {
        var schemaPart = pre.DefaultSchema == post.DefaultSchema ? "" : $"DEFAULT_SCHEMA = {post.DefaultSchema}";
        var loginPart = pre.Login == post.Login ? "" : $"LOGIN = {post.Login}";
        var languagePart = pre.DefaultLanguage == post.DefaultLanguage ? "" : $"DEFAULT_LANGUAGE = {post.DefaultLanguage}";

        var mods = string.Join(", ", new string[] { schemaPart, loginPart, languagePart, }.Where(s => !string.IsNullOrWhiteSpace(s)));

        return $"ALTER USER {post.Identifier} WITH {mods}";
    }

    protected override IEnumerable<DbObject> Selector(IDatabase db)
        => db.Users;
}
