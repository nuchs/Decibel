using Mast;
using Patchwerk;

namespace Tests.Patchwerk;

internal class BasePatchwerkTest
{
    protected Differ sut = new();
    private DbBuilder db = new();

    public IDatabase MakeDb(string script="")
        => db.AddFromTsqlScript(script).Build();

    [SetUp]
    public void Setup() => sut = new();
}
