using Lindt.Checks;
using Log;
using Mast;

namespace Lindt;

public class Linter
{
    private static readonly ILog Log = LoggerFactory.CreateLogger<Linter>();

    private readonly Dictionary<string, ICheck> checks = new();

    public Linter()
    {
        checks[nameof(Unresolved)] = new Unresolved();
        checks[nameof(Unreferenced)] = new Unreferenced();
    }

    public bool this[string name]
    {
        get => checks[name].Enabled;
        set
        {
            var setting = value ? "enabled" : "disabled";
            Log.Info($"Check {name} is {setting}");
            checks[name].Enabled = value;
        }
    }

    public ResultSet Run(IDatabase db)
    {
        ResultSet results = new();

        foreach (var (name, check) in checks)
        {
            if (check.Enabled)
            {
                Log.Info($"Running check {name}");
                results.AddResults(check.Check(db));
            }
        }

        return results;
    }
}
