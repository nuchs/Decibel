using Mast.Parsing;

namespace Mast;

public class DbBuilder
{
    private readonly ScriptParser parser;
    private readonly Database db = new();

    public DbBuilder()
    {
        parser = new(db);
    }

    public IDatabase Build()
    {
        foreach (var item in db.NameMap.Values)
        {
            item.CrossReference(db);
        }

        return db;
    }

    public DbBuilder AddFromTsqlScript(string script)
    {
        parser.Parse(script);
        return this;
    }
}
