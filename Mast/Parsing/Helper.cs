using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Parsing;

internal static class Helper
{
    internal static IEnumerable<TSqlParserToken> FragmentStream(this TSqlFragment target)
        => target.ScriptTokenStream.Take(target.FirstTokenIndex..(target.LastTokenIndex + 1));
}
