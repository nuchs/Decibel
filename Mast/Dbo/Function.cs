using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mast.Dbo;
public class Function
{
    public string Content;
    public List<object> ReferencedBy = new();
    public List<object> References = new();
    public string Name;
    public string Schema;
    public List<Parameter> Parameters = new();
    public object ReturnType;

    public Function(CreateFunctionStatement node)
    {
        var tokenValues = node.ScriptTokenStream.Select(t => t.Text);
        Content = string.Join(string.Empty, tokenValues);

        Name = node.Name.BaseIdentifier.Value;
        Schema = node.Name.SchemaIdentifier.Value;

            var rtnTokens = 
            node.ScriptTokenStream
                .Take(node.ReturnType.FirstTokenIndex .. (node.ReturnType.FirstTokenIndex+1))
                .Select(f => f.Text);
        ReturnType = string.Join("", rtnTokens).Trim();

        foreach (var paramDef in node.Parameters)
        {
            Parameters.Add(new Parameter(paramDef));
        }
    }

    public string GeneratePatch(Function target)
    {
        return "";
    }
}
