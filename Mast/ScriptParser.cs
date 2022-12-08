using Log;
using Mast.Dbo;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast;

public class ScriptParser
{
    private static ILog Log = LoggerFactory.CreateLogger<ScriptParser>();

    public void Parse(Database db, string content)
    {
        var tree = MakeAbstractSyntaxTree(content);
        AddToDb(tree, db);
    }

    private static void AddToDb(TSqlFragment tree, Database db)
    {
        try
        {
            DefinitionVisitor visitor = new(db);
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
                Log.Error($"Error {err.Number} : [{err.Line}, {err.Offset}] {err.Message}");
            }

            throw new InvalidOperationException("Parse error");
        }

        return tree;
    }
}
