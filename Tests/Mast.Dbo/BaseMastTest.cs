using Mast;
using Mast.Dbo;

namespace Tests.Mast.Dbo;

public class BaseMastTest
{
    protected Database db { get; private set; } = new();

    protected ScriptParser parser { get; } = new();

    [SetUp]
    public void Setup() => db = new();
}
