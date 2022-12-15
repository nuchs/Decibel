using Log;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Parsing;

internal sealed class ScriptParser
{
    private static readonly ILog Log = LoggerFactory.CreateLogger<ScriptParser>();
    private readonly Database db;

    public ScriptParser(Database db) => this.db = db;

    public void Parse(string content)
    {
        var tree = MakeAbstractSyntaxTree(content);
        AddObjectsToDb(tree);
        ExtractReferences(tree);
    }

    private void AddObjectsToDb(TSqlFragment tree)
        => VisitTree(tree, new DefinitionVisitor(db), "Failed to build db representation");

    private void ExtractReferences(TSqlFragment tree)
        => VisitTree(tree, new ReferenceVisitor(db), "Failed to extract references");

    private TSqlFragment MakeAbstractSyntaxTree(string content)
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
