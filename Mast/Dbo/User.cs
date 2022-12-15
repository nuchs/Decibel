using Mast.Parsing;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class User : DbObject
{
    public User(CreateUserStatement user)
        : base(user)
    {
        Name = GetName(user);
        Login = GetLogin(user);
        Password = GetPassword(user);
        DefaultSchema = GetSchema(user);
        DefaultLanguage = GetLanguage(user);
        Sid = GetSid(user);
    }

    public string DefaultLanguage { get; }

    public string DefaultSchema { get; }

    public string Login { get; }

    public string Password { get; }

    public string Sid { get; }

    internal override void CrossReference(Database db) => throw new NotImplementedException();
    private string GetIdentifierOption(PrincipalOptionKind kind, IEnumerable<PrincipalOption> options)
        => options
            .Where(o => o.OptionKind == kind)
            .OfType<IdentifierPrincipalOption>()
            .Select(o => GetId(o.Identifier))
            .FirstOrDefault() ?? string.Empty;

    private string GetLanguage(CreateUserStatement user)
        => GetIdentifierOption(PrincipalOptionKind.DefaultLanguage, user.UserOptions);

    private string GetLiteralOption(PrincipalOptionKind kind, IEnumerable<PrincipalOption> options)
        => options
          .Where(o => o.OptionKind == kind)
          .OfType<LiteralPrincipalOption>()
          .Select(o => o.Value.Value)
          .FirstOrDefault() ?? string.Empty;

    private string GetLogin(CreateUserStatement user)
        => GetId(user.UserLoginOption?.Identifier);

    private string GetName(CreateUserStatement user)
        => GetId(user.Name);

    private string GetPassword(CreateUserStatement user)
        => GetLiteralOption(PrincipalOptionKind.Password, user.UserOptions);

    private string GetSchema(CreateUserStatement user)
        => GetIdentifierOption(PrincipalOptionKind.DefaultSchema, user.UserOptions);

    private string GetSid(CreateUserStatement user)
        => GetLiteralOption(PrincipalOptionKind.Sid, user.UserOptions);
}
