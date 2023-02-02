using Mast;
using Patchwerk;

namespace Tests.Patchwerk;

internal class BasePatchwerkTest
{
    protected Differ sut = new();
    private DbBuilder db = new();

    public IDatabase MakeDb(params string[] scripts)
    {
        foreach (var script in scripts)
        {
            db.AddFromTsqlScript(script);
        }

        return db.Build();
    }

    [SetUp]
    public void Setup() => sut = new();
}
