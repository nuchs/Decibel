using Mast;
using Mast.Parsing;

namespace Tests.Mast;

public class BaseMastTest
{
    protected Database db { get; private set; } = new();

    [SetUp]
    public void Setup() => db = new();
}
