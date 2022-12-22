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
    }

    private void AddObjectsToDb(TSqlFragment tree)
    {
        try
        {
            tree.Accept(new DefinitionVisitor(db));
        }
        catch (Exception e)
        {
            Log.Error($"\"Failed to build db representation\"\n{e}");
            throw;
        }
    }

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
}
