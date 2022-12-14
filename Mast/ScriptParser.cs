using Log;
using Mast.Dbo;
using Mast.Parsing;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast;

public class ScriptParser
{
    private static ILog Log = LoggerFactory.CreateLogger<ScriptParser>();

    public void Parse(Database db, string content)
    {
        var tree = MakeAbstractSyntaxTree(content);
        AddObjectsToDb(tree, db);
        BuildReferences(tree, db);
    }

    private static void AddObjectsToDb(TSqlFragment tree, Database db)
        => VisitTree(tree, new DefinitionVisitor(db), "Failed to build db representation");

    private static void BuildReferences(TSqlFragment tree, Database db)
        => VisitTree(tree, new ReferenceVisitor(db), "Failed to cross reference db objects");

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

    private static void VisitTree(TSqlFragment tree, TSqlFragmentVisitor visitor, string errMsg)
    {
        try
        {
            tree.Accept(visitor);
        }
        catch (Exception e)
        {
            Log.Error($"{errMsg}\n{e}");
            throw;
        }
    }
}
