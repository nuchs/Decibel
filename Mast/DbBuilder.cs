using Mast.Parsing;

namespace Mast;

public class DbBuilder
{
    private readonly ScriptParser parser;
    private readonly CrossReferencer referencer;
    private readonly Database db = new();

    public DbBuilder()
    {
        parser = new(db);
        referencer = new(db);
    }

    public IDatabase Build()
    {
        referencer.Run();
        return db;
    }

    public DbBuilder AddFromTsqlScript(string script)
    {
        parser.Parse(script);
        return this;
    }
}
