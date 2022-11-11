using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Text;

namespace Mast.Dbo;

public class StoredProcedure
{
    public string Content;
    public List<object> ReferencedBy = new();
    public List<object> References = new();
    public string Name;
    public string Schema;
    public List<object> Parameters = new();

    public StoredProcedure(CreateProcedureStatement node)
    {
        var tokenValues = node.ScriptTokenStream.Select(t => t.Text);
        Content = string.Join(string.Empty, tokenValues);

        Name = node.ProcedureReference.Name.BaseIdentifier.Value;
        Schema = node.ProcedureReference.Name.SchemaIdentifier.Value;
    
        foreach (var paramDef in node.Parameters)
        {
            Parameters.Add(new Parameter(paramDef));
        }
    }

    public string GeneratePatch(StoredProcedure target)
    {
        if (target.Content == Content)
            return "";

        StringBuilder patch = new();
        var (drop, create) = Recreate();
        patch.AppendLine(drop);
        patch.AppendLine(create);

        return patch.ToString();
    }

    public (string drop, string create) Recreate()
    {
        var drop = $@"IF dbo.StoredProcExists({Schema}.{Name}) = 1
    DROP PROCEDURE {Schema}.{Name}
END IF";

        return (drop, Content);
    }
}
