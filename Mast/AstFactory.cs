using Log;
using Mast.Dbo;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast;

public class AstFactory
{
    private static ILog Log = LoggerFactory.CreateLogger<AstFactory>();

    public void Parse(Database db, string content)
    {
        TSql150Parser parser = new(true, SqlEngineType.All);
        var tree = parser.Parse(new StringReader(content), out var errors);

        if (errors.Any())
        {
            Log.Error($"Failed to parse\n{content}");

            foreach (var err in errors)
            {
                Log.Error(err.ToString());
            }

            return;
        }

        Visitor visitor = new(db);
        tree.Accept(visitor);
    }
}
