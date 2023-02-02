using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class User : DbObject
{
    public User(CreateUserStatement user)
        : base(user)
    {
        Identifier = FullyQualifiedName.FromName(GetName(user));
        Login = GetLogin(user);
        DefaultSchema = GetSchema(user);
        DefaultLanguage = GetLanguage(user);
    }

    public string DefaultLanguage { get; }

    public string DefaultSchema { get; }

    public string Login { get; }

    private string GetIdentifierOption(PrincipalOptionKind kind, IEnumerable<PrincipalOption> options)
        => options
            .Where(o => o.OptionKind == kind)
            .OfType<IdentifierPrincipalOption>()
            .Select(o => GetId(o.Identifier))
            .FirstOrDefault() ?? string.Empty;

    private string GetLanguage(CreateUserStatement user)
        => GetIdentifierOption(PrincipalOptionKind.DefaultLanguage, user.UserOptions);

    private string GetLogin(CreateUserStatement user)
        => GetId(user.UserLoginOption?.Identifier);

    private string GetName(CreateUserStatement user)
        => GetId(user.Name);

    private string GetSchema(CreateUserStatement user)
        => GetIdentifierOption(PrincipalOptionKind.DefaultSchema, user.UserOptions);
}
