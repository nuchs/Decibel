using Log;
using Mast.Dbo;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast;

public class AstFactory
{
    private static ILog Log = LoggerFactory.CreateLogger<AstFactory>();

    public void Parse(Database db, string content)
    {
        var tree = MakeAbstractSyntaxTree(content);
        AddToDb(tree, db);
    }

    private static void AddToDb(TSqlFragment tree, Database db)
    {
        try
        {
            Visitor visitor = new(db);
            tree.Accept(visitor);
        }
        catch (Exception e)
        {
            Log.Error($"Failed to build db object\n{e}");
            throw;
        }
    }

    private static TSqlFragment MakeAbstractSyntaxTree(string content)
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

            throw new InvalidOperationException("Parse error");
        }

        return tree;
    }
}
