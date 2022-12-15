using Mast;

namespace Tests.Mast;

public class BaseMastTest
{
    protected DbBuilder dbBuilder { get; private set; } = new();

    [SetUp]
    public void Setup() => dbBuilder = new();
}
