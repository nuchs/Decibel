using Lindt;
using Mast;

namespace Tests.Lindt;

public class UnresolvedTests
{
    private Linter sut = new();

    [SetUp]
    public void SetUp()
    {
        sut = new();
        sut["Unreferenced"] = false;
    }

    [Test]
    public void UnresolvedFunction()
    {

    }
    [Test]
    public void UnresolvedScalarType()
    {

    }
    [Test]
    public void UnresolvedSchema()
    {

    }
    [Test]
    public void UnresolvedTable()
    {

    }

    [Test]
    public void UnresolvedTableColumn()
    {

    }

    [Test]
    public void UnresolvedTableAliasedColumn()
    {

    }
    [Test]
    public void UnresolvedTableType()
    {

    }

    [Test]
    public void UnresolvedTableTypeColumn()
    {

    }

    [Test]
    public void UnresolvedTableTypeAliasedColumn()
    {

    }

    [Test]
    public void UnresolvedView()
    {

    }

    [Test]
    public void UnresolvedViewColumn()
    {

    }

    [Test]
    public void UnresolvedViewAliasedColumn()
    {

    }

    [Test]
    public void UnresolvedProcedure()
    {

    }
}
