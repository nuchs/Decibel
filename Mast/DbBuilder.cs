using Mast.Parsing;

namespace Mast;

public class DbBuilder
{
    private ScriptParser parser;
    private Database db;

    public DbBuilder()
    {
        db = new();
        parser = new(db);
    }

    public IDatabase Build()
    {
        foreach (var item in db.Values)
        {
            item.CrossReference(db);
        }

        var built = db;
        db = new();
        parser = new(db);

        return built;
    }

    public DbBuilder AddFromTsqlScript(string script)
    {
        parser.Parse(script);
        return this;
    }
}
